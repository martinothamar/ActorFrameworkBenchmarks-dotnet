using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ActorFrameworkBenchmarks.CustomActor
{
    public interface IActor
    {
        string Id { get; }
    }

    public abstract class ActorBase : IActor
    {
        public string Id { get; internal set; }
    }

    public readonly struct PingRequest
    {
        public readonly string Ping;

        public PingRequest(string ping)
        {
            Ping = ping;
        }
    }

    public readonly struct PingResponse
    {
        public readonly string Pong;

        public PingResponse(string pong)
        {
            Pong = pong;
        }
    }

    public class PingRequestClass
    {
        public readonly string Ping;

        public PingRequestClass(string ping)
        {
            Ping = ping;
        }
    }

    public class PingResponseClass
    {
        public readonly string Pong;

        public PingResponseClass(string pong)
        {
            Pong = pong;
        }
    }

    public interface IPingCustomActor : IActor
    {
        Task<string> PingAsync(string ping);
        string Ping(string ping);
    }

    public class PingCustomActor : ActorBase, IPingCustomActor
    {
        public string Ping(string ping)
        {
            return "pong";
        }

        public Task<string> PingAsync(string ping)
        {
            return Task.FromResult("pong");
        }
    }

    public interface IContext
    {
        T GetActor<T>(string id) where T : class, IActor;
    }

    public class CustomContext : IContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<(string Id, Type ActorType), IActor> _directory;

        private readonly IReadOnlyDictionary<Type, Type> _actorTypeMapping;
        private readonly Assembly _generatedAssembly;

        public CustomContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _directory = new ConcurrentDictionary<(string, Type), IActor>();

            var actorBaseType = typeof(ActorBase);
            var actorInterfaceType = typeof(IActor);

            var assemblyTypes = typeof(CustomContext).Assembly.DefinedTypes;
            var actorInterfaceTypes = assemblyTypes.Where(t => t != actorInterfaceType && t.IsInterface && actorInterfaceType.IsAssignableFrom(t)).ToArray();
            var actorTypes = assemblyTypes.Where(t => t != actorBaseType && actorBaseType.IsAssignableFrom(t)).ToArray();

            _actorTypeMapping = actorInterfaceTypes.ToDictionary(t => t.AsType(), t => actorTypes.Single(at => t.IsAssignableFrom(at)).AsType());

            var awaitableTypes = new[] { typeof(Task), typeof(ValueTask), typeof(Task<>), typeof(ValueTask<>) };

            var classes = actorTypes.Select(actorType =>
            {
                var @namespace = SF.NamespaceDeclaration(SF.ParseName(actorType.Namespace)).NormalizeWhitespace();

                var interfaceType = actorInterfaceTypes.Single(it => it.IsAssignableFrom(actorType));

                var @class = SF.ClassDeclaration(actorType.Name + "Generated");

                @namespace = @namespace.AddUsings(SF.UsingDirective(SF.ParseName("System.Threading")));

                @class = @class.AddModifiers(SF.Token(SyntaxKind.PublicKeyword));

                @class = @class.AddBaseListTypes(
                    SF.SimpleBaseType(SF.ParseTypeName("ActorBase")),
                    SF.SimpleBaseType(SF.ParseTypeName(interfaceType.Name)));

                var lockVariableDeclaration = SF.VariableDeclaration(SF.ParseTypeName("SemaphoreSlim"))
                    .AddVariables(SF.VariableDeclarator("_lock"));
                var lockFieldDeclaration = SF.FieldDeclaration(lockVariableDeclaration)
                    .AddModifiers(SF.Token(SyntaxKind.PrivateKeyword))
                    .AddModifiers(SF.Token(SyntaxKind.ReadOnlyKeyword));

                var constructorDeclaration = SF.ConstructorDeclaration(actorType.Name + "Generated")
                    .AddParameterListParameters(
                        SF.Parameter(SF.Identifier("actor")).WithType(SF.ParseTypeName(actorType.FullName))
                    )
                    .WithBody(SF.Block(
                        SF.ParseStatement("_actor = actor;"),
                        SF.ParseStatement("_lock = new SemaphoreSlim(1, 1);")
                    ))
                    .AddModifiers(SF.Token(SyntaxKind.PublicKeyword));

                var actorVariableDeclaration = SF.VariableDeclaration(SF.ParseTypeName(actorType.FullName))
                    .AddVariables(SF.VariableDeclarator("_actor"));
                var actorFieldDeclaration = SF.FieldDeclaration(actorVariableDeclaration)
                    .AddModifiers(SF.Token(SyntaxKind.PrivateKeyword))
                    .AddModifiers(SF.Token(SyntaxKind.ReadOnlyKeyword));

                @class = @class.AddMembers(lockFieldDeclaration);
                @class = @class.AddMembers(actorFieldDeclaration);

                @class = @class.AddMembers(constructorDeclaration);

                var actorMethods = interfaceType.GetMethods().Where(m => m.IsPublic && !m.IsStatic);
                foreach (var method in actorMethods)
                {
                    var isAwaitable = awaitableTypes.Any(at => at.IsAssignableFrom(method.ReturnType));

                    StatementSyntax syntax = null;
                    if (isAwaitable)
                        syntax = SF.ParseStatement($"return await _actor.{method.Name}({string.Join(',', method.GetParameters().Select(p => p.Name))});");
                    else
                        syntax = SF.ParseStatement($"return _actor.{method.Name}({string.Join(',', method.GetParameters().Select(p => p.Name))});");

                    string returnTypeName = GetFriendlyName(method.ReturnType);

                    var acquireLockStatement = isAwaitable ? SF.ParseStatement("await _lock.WaitAsync();") : SF.ParseStatement("_lock.Wait();");

                    var methodDeclaration = SF.MethodDeclaration(SF.ParseTypeName(returnTypeName), method.Name)
                       .AddModifiers(SF.Token(SyntaxKind.PublicKeyword))
                       .AddModifiers(isAwaitable ? new[] { SF.Token(SyntaxKind.AsyncKeyword) } : new SyntaxToken[0])
                       .AddParameterListParameters(method.GetParameters().Select(p => SF.Parameter(SF.Identifier(p.Name)).WithType(SF.ParseTypeName(p.ParameterType.FullName))).ToArray())
                       .WithBody(SF.Block(acquireLockStatement, SF.TryStatement(SF.Token(SyntaxKind.TryKeyword), SF.Block(syntax), default, SF.FinallyClause(SF.Block(SF.ParseStatement("_lock.Release();"))))));

                    @class = @class.AddMembers(methodDeclaration);
                }

                @namespace = @namespace.AddMembers(@class);

                var code = @namespace
                    .NormalizeWhitespace()
                    .ToFullString();

                return code;
            }).ToArray();

            var tree = SF.ParseSyntaxTree(string.Join('\n', classes));

            var assemblies =
                AppDomain.CurrentDomain.GetAssemblies()
                         .Where(asm => !asm.IsDynamic && !string.IsNullOrWhiteSpace(asm.Location))
                         .Select(asm => MetadataReference.CreateFromFile(asm.Location))
                         .Cast<MetadataReference>()
                         .ToArray();

            // Generate the code.
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation =
                CSharpCompilation.Create("CustomActors.dll")
                                 .AddSyntaxTrees(tree)
                                 .AddReferences(assemblies)
                                 .WithOptions(options);
            byte[] array = null;

            using (var stream = new MemoryStream())
            {
                // emit result into a stream
                var emitResult = compilation.Emit(stream);

                if (!emitResult.Success)
                {
                    // if not successful, throw an exception
                    Diagnostic firstError =
                        emitResult
                            .Diagnostics
                            .FirstOrDefault
                            (
                                diagnostic =>
                                    diagnostic.Severity == DiagnosticSeverity.Error
                            );

                    throw new Exception(firstError?.GetMessage());
                }

                // get the byte array from a stream
                array = stream.ToArray();
            }
            _generatedAssembly = Assembly.Load(array);
        }

        private static string GetFriendlyName(Type type)
        {
            string friendlyName = type.FullName;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; ++i)
                {
                    string typeParamName = GetFriendlyName(typeParameters[i]);
                    friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
                }
                friendlyName += ">";
            }

            return friendlyName;
        }

        public T GetActor<T>(string id) where T : class, IActor
        {
            (string Id, Type ActorInterfaceType) key = (id, typeof(T));

            var actor = _directory.GetOrAdd(key, _ =>
            {
                var implType = _actorTypeMapping[key.ActorInterfaceType];
                var actorInstance = ActivatorUtilities.CreateInstance(_serviceProvider, implType);
                var b = actorInstance as ActorBase;
                b.Id = id;
                var generatedProxyType = _generatedAssembly.DefinedTypes.Single(t => t.IsClass && t.Name.StartsWith(implType.Name));
                var generatedProxyInstance = Activator.CreateInstance(generatedProxyType, actorInstance) as T;
                return generatedProxyInstance;
            });

            return actor as T;
        }
    }
}

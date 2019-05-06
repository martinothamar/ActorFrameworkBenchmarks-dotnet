using ActorFrameworkBenchmarks.CustomActor;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks
{
    internal static class Program
    {
        static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

        //static async Task Main()
        //{
        //    var services = new ServiceCollection().BuildServiceProvider();
        //    var context = new CustomContext(services);

        //    var actor = context.GetActor<IPingCustomActor>("1");

        //    var pong = await actor.PingAsync("ping");
        //    Console.WriteLine(pong);
        //}
    }
}

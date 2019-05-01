using ActorFrameworkBenchmarks.Akka.NET;
using Akka.Actor;
using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks
{
    [InProcess, MemoryDiagnoser]
    public class AkkaBenchmarks
    {
        private ActorSystem _system;
        private IActorRef _actor;

        [Params(1, 100, 1_000)]
        public int Concurrency { get; set; }

        private IActorRef[] _actors;

        [GlobalSetup]
        public void Setup()
        {
            _system = ActorSystem.Create("System");

            if (Concurrency > 1)
            {
                _actors = Enumerable.Range(0, Concurrency).Select(i => _system.ActorOf<PingActor>(i.ToString())).ToArray();
            }
            else
            {

                _actor = _system.ActorOf<PingActor>("1");
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            if (_actor != null)
            {
                _actor.GracefulStop(TimeSpan.FromSeconds(5));
                _actor = null;
            }
            if (_actors != null)
            {
                for (int i = 0; i < _actors.Length; i++)
                    _actors[i].GracefulStop(TimeSpan.FromSeconds(5));
                _actors = null;
            }
        }

        [Benchmark]
        public Task AkkaPing()
        {
            if (Concurrency == 1)
                return _actor.Ask("ping", CancellationToken.None);

            var tasks = new Task<string>[Concurrency];
            for (int i = 0; i < Concurrency; i++)
            {
                tasks[i] = _actors[i].Ask<string>("ping", CancellationToken.None);
            }
            return Task.WhenAll(tasks);
        }
    }
}

using ActorFrameworkBenchmarks.CustomActor;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks
{
    [InProcess, MemoryDiagnoser]
    public class CustomActorBenchmarks
    {
        private CustomContext _context;
        private IPingCustomActor _actor;

        [Params(1, 100, 1_000)]
        public int Concurrency { get; set; }

        private IPingCustomActor[] _actors;

        [GlobalSetup]
        public void Setup()
        {
            _context = new CustomContext(new ServiceCollection().BuildServiceProvider());

            if (Concurrency > 1)
            {
                _actors = Enumerable.Range(0, Concurrency).Select(i => _context.GetActor<IPingCustomActor>(i.ToString())).ToArray();
            }
            else
            {
                _actor = _context.GetActor<IPingCustomActor>("1");
            }
        }

        [Benchmark]
        public Task CustomActorPing()
        {
            if (Concurrency == 1)
                return _actor.PingAsync("ping");

            var tasks = new Task<string>[Concurrency];
            for (int i = 0; i < Concurrency; i++)
            {
                tasks[i] = _actors[i].PingAsync("ping");
            }
            return Task.WhenAll(tasks);
        }
    }
}

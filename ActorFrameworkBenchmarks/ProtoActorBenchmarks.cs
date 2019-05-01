using ActorFrameworkBenchmarks.ProtoActor;
using BenchmarkDotNet.Attributes;
using Proto;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks
{
    [InProcess, MemoryDiagnoser]
    public class ProtoActorBenchmarks
    {
        private RootContext _context;
        private Props _props;
        private PID _actor;
        private TimeSpan _timeout;

        [Params(1, 100, 1_000)]
        public int Concurrency { get; set; }

        private PID[] _actors;

        [GlobalSetup]
        public void Setup()
        {
            _context = new RootContext();

            _props = Props.FromProducer(() => new PingActor());
            _timeout = TimeSpan.FromSeconds(5);

            if (Concurrency > 1)
            {
                _actors = Enumerable.Range(0, Concurrency).Select(i => _context.SpawnNamed(_props, i.ToString())).ToArray();
            }
            else
            {
                _actor = _context.SpawnNamed(_props, "1");
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            if (_actor != null)
            {
                _actor.Stop();
                _actor = null;
            }
            if (_actors != null)
            {
                for (int i = 0; i < _actors.Length; i++)
                    _actors[i].Stop();
                _actors = null;
            }
        }

        [Benchmark]
        public Task ProtoActorPing()
        {
            if (Concurrency == 1)
                return _context.RequestAsync<string>(_actor, "ping", _timeout);

            var tasks = new Task<string>[Concurrency];
            for (int i = 0; i < Concurrency; i++)
            {
                tasks[i] = _context.RequestAsync<string>(_actors[i], "ping", _timeout);
            }
            return Task.WhenAll(tasks);
        }
    }
}

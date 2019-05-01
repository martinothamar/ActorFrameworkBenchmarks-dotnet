using ActorFrameworkBenchmarks.ProtoActor;
using BenchmarkDotNet.Attributes;
using Proto;
using Proto.Mailbox;
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

            _props = Props.FromProducer(() => new PingActor())
                .WithMailbox(() => BoundedMailbox.Create(2048));
            _actor = _context.Spawn(_props);
            _timeout = TimeSpan.FromSeconds(5);

            if (Concurrency > 1)
            {
                _actors = Enumerable.Range(0, Concurrency).Select(_ => _context.Spawn(_props)).ToArray();
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

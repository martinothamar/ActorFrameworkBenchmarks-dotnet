using ActorFrameworkBenchmarks.CustomActor;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks
{
    internal class BaseActor
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly object _lockable = new object();

        public async Task<string> PingAsync(string ping)
        {
            await _lock.WaitAsync();
            try
            {
                return "pong";
            }
            finally
            {
                _lock.Release();
            }
        }

        public string Ping(string ping)
        {
            lock (_lockable)
            {
                return "pong";
            }
        }

        public PingResponse Ping(in PingRequest ping)
        {
            lock (_lockable)
            {
                return new PingResponse("pong");
            }
        }

        public PingRequestClass Ping(PingRequestClass ping)
        {
            lock (_lockable)
            {
                return new PingRequestClass("pong");
            }
        }
    }

    [InProcess, MemoryDiagnoser]
    public class BaseBenchmark
    {
        private BaseActor _actor;

        private IContext _customContext;
        private IPingCustomActor _customActor;

        [Params(1, 10_000)]
        public int Count { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _actor = new BaseActor();

            var services = new ServiceCollection();

            _customContext = new CustomContext(services.BuildServiceProvider());
            _customActor = _customContext.GetActor<IPingCustomActor>("1");
        }

        [Benchmark]
        public string BasePing()
        {
            string result = null;
            for (int i = 0; i < Count; i++)
                result = _actor.Ping("ping");
            return result;
        }


        [Benchmark]
        public string CustomPing()
        {
            string result = null;
            for (int i = 0; i < Count; i++)
                result = _customActor.Ping("ping");
            return result;
        }
    }
}

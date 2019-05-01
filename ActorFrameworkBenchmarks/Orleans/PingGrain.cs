using Orleans;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks.Orleans
{
    public interface IPingGrain : IGrainWithStringKey
    {
        Task<string> Ping(string ping);
    }

    public class PingGrain : Grain, IPingGrain
    {
        public Task<string> Ping(string ping)
        {
            return Task.FromResult("pong");
        }
    }
}

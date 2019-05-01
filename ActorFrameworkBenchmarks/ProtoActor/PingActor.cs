using Proto;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks.ProtoActor
{
    public class PingActor : IActor
    {
        public Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case string _:
                    context.Send(context.Sender, "pong");
                    break;
            }
            return Task.CompletedTask;
        }
    }
}

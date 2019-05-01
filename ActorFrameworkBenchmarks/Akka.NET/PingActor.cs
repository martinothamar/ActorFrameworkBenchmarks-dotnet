using Akka.Actor;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks.Akka.NET
{
    public class PingActor : ReceiveActor
    {
        public PingActor()
        {
            // Tell the actor to respond to the Greet message
            ReceiveAsync<string>(ping =>
            {
                Sender.Tell("pong");
                return Task.CompletedTask;
            });
        }
    }
}

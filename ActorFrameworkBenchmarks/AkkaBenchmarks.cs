using ActorFrameworkBenchmarks.Akka.NET;
//using Akka.Actor;
using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks
{
    //[InProcess, MemoryDiagnoser]
    //public class AkkaBenchmarks
    //{
    //    private ActorSystem _system;
    //    private IActorRef _actor;

    //    [Params(1, 100, 1_000)]
    //    public int Concurrency { get; set; }

    //    private IActorRef[] _actors;

    //    [GlobalSetup]
    //    public void Setup()
    //    {
    //        _system = ActorSystem.Create("System");

    //        _actor = _system.ActorOf<PingActor>("1");

    //        if (Concurrency > 1)
    //        {
    //            _actors = Enumerable.Range(0, Concurrency).Select(i => _system.ActorOf<PingActor>(i.ToString())).ToArray();
    //        }
    //    }

    //    [Benchmark]
    //    public Task AkkaPing()
    //    {
    //        if (Concurrency == 1)
    //            return _actor.Ask("ping", CancellationToken.None);

    //        var tasks = new Task<string>[Concurrency];
    //        for (int i = 0; i < Concurrency; i++)
    //        {
    //            tasks[i] = _actors[i].Ask<string>("ping", CancellationToken.None);
    //        }
    //        return Task.WhenAll(tasks);
    //    }
    //}
}

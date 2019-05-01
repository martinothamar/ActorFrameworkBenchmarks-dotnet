using ActorFrameworkBenchmarks.Orleans;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks
{
    [InProcess, MemoryDiagnoser]
    public class OrleansBenchmarks
    {
        private IHost _host;
        private IPingGrain _grain;

        [Params(1, 100, 1_000)]
        public int Concurrency { get; set; }

        public IPingGrain[] _grains;

        [GlobalSetup]
        public void Setup()
        {
            _host = new HostBuilder()
                .UseOrleans(builder =>
                {
                    builder
                        // START workaround
                        .ConfigureDefaults()
                        .ConfigureServices((ctx, services) =>
                        {
                            var toRemove = services.Where(s => s.ServiceType == typeof(IPostConfigureOptions<SiloOptions>)).ToList();
                            foreach (var definition in toRemove)
                            {
                                services.Remove(definition);
                            }
                        })
                        .Configure<SiloOptions>(s => s.SiloName = $"Silo_{Guid.NewGuid().ToString("N").Substring(0, 5)}")
                        // END workaround
                        .UseLocalhostClustering()
                        .Configure<ClusterOptions>(options =>
                        {
                            options.ClusterId = "dev";
                            options.ServiceId = "HelloWorldApp";
                        })
                        .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                        .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(PingGrain).Assembly));
                })
                .ConfigureServices(services =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options =>
                    {
                        options.SuppressStatusMessages = true;
                    });
                })
                .Build();

            _host.Start();

            var client = _host.Services.GetService<IClusterClient>();

            if (Concurrency > 1)
            {
                _grains = Enumerable.Range(0, Concurrency).Select(i => client.GetGrain<IPingGrain>(i.ToString())).ToArray();

                var tasks = new Task[_grains.Length];
                for (int i = 0; i < _grains.Length; i++)
                    tasks[i] = _grains[i].Ping("ping");
                Task.WhenAll(tasks).GetAwaiter().GetResult();
            }
            else
            {
                _grain = client.GetGrain<IPingGrain>("1");

                // Force activate so that all calls are "hot"
                _grain.Ping("ping").GetAwaiter().GetResult();
            }
        }


        [GlobalCleanup]
        public void Cleanup()
        {
            _host.StopAsync().GetAwaiter().GetResult();
        }

        [Benchmark]
        public Task OrleansPing()
        {
            if (Concurrency == 1)
                return _grain.Ping("ping");

            var tasks = new Task<string>[Concurrency];
            for (int i = 0; i < Concurrency; i++)
            {
                tasks[i] = _grains[i].Ping("ping");
            }
            return Task.WhenAll(tasks);
        }
    }
}

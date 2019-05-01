using BenchmarkDotNet.Running;
using System;
using System.Threading.Tasks;

namespace ActorFrameworkBenchmarks
{
    internal static class Program
    {
        static void Main(string[] args) => BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}

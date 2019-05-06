## Actor framework benchmarks for dotnet

Benchmarks for actor frameworks in .NET Core 3.0 using BenchmarkDotNet.
This currently only contains a simple ping-pong benchmark where 1, 100 or 1000 requests are dispatched at once.

Results:

1. Proto.Actor
2. Akka.Net
3. Orleans

These frameworks are very different in purpose and vision so Orleans being the slowest here is expected.

### CustomActor

``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview4-011223
  [Host] : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|          Method | Concurrency |         Mean |        Error |       StdDev |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
|---------------- |------------ |-------------:|-------------:|-------------:|--------:|-------:|------:|----------:|
| **CustomActorPing** |           **1** |     **155.7 ns** |     **1.287 ns** |     **1.141 ns** |  **0.0114** |      **-** |     **-** |     **216 B** |
| **CustomActorPing** |         **100** |  **20,494.7 ns** |   **180.068 ns** |   **168.436 ns** |  **1.2817** |      **-** |     **-** |   **24160 B** |
| **CustomActorPing** |        **1000** | **211,043.0 ns** | **2,585.495 ns** | **2,291.974 ns** | **12.6953** | **1.2207** |     **-** |  **240160 B** |


### Proto.Actor

``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview4-011223
  [Host] : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|         Method | Concurrency |         Mean |      Error |     StdDev |    Gen 0 | Gen 1 | Gen 2 |  Allocated |
|--------------- |------------ |-------------:|-----------:|-----------:|---------:|------:|------:|-----------:|
| **ProtoActorPing** |           **1** |     **3.117 us** |  **0.0350 us** |  **0.0292 us** |   **0.0496** |     **-** |     **-** |    **1.13 KB** |
| **ProtoActorPing** |         **100** |   **228.313 us** |  **2.9123 us** |  **2.7242 us** |   **9.0332** |     **-** |     **-** |  **114.98 KB** |
| **ProtoActorPing** |        **1000** | **2,890.304 us** | **57.6130 us** | **80.7654 us** | **125.0000** |     **-** |     **-** | **1148.59 KB** |


### Akka.Net

``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview4-011223
  [Host] : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|   Method | Concurrency |         Mean |      Error |     StdDev |    Gen 0 |    Gen 1 | Gen 2 |  Allocated |
|--------- |------------ |-------------:|-----------:|-----------:|---------:|---------:|------:|-----------:|
| **AkkaPing** |           **1** |     **8.340 us** |  **0.1650 us** |  **0.2258 us** |   **0.0763** |        **-** |     **-** |    **1.35 KB** |
| **AkkaPing** |         **100** |   **271.244 us** |  **2.3312 us** |  **2.1806 us** |  **15.6250** |        **-** |     **-** |  **137.48 KB** |
| **AkkaPing** |        **1000** | **3,607.065 us** | **70.3277 us** | **80.9895 us** | **355.4688** | **171.8750** |     **-** | **1374.43 KB** |


### Orleans

``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview4-011223
  [Host] : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|      Method | Concurrency |        Mean |      Error |     StdDev |    Gen 0 |   Gen 1 | Gen 2 |  Allocated |
|------------ |------------ |------------:|-----------:|-----------:|---------:|--------:|------:|-----------:|
| **OrleansPing** |           **1** |    **12.68 us** |  **0.2878 us** |  **0.2826 us** |   **0.1068** |       **-** |     **-** |    **1.72 KB** |
| **OrleansPing** |         **100** |   **665.92 us** | **10.3597 us** |  **9.1836 us** |  **13.6719** |       **-** |     **-** |  **173.57 KB** |
| **OrleansPing** |        **1000** | **7,587.61 us** | **59.0190 us** | **55.2064 us** | **375.0000** | **54.6875** |     **-** | **1742.26 KB** |

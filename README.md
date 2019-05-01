### Actor framework benchmarks for dotnet

Benchmarks for actor frameworks in .NET Core 3.0 using BenchmarkDotNet.


#### Orleans

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



#### Proto.Actor

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

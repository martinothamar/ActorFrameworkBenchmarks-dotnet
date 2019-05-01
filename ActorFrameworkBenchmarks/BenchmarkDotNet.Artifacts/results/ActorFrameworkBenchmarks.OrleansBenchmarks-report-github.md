``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview4-011223
  [Host] : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|      Method | Concurrency |        Mean |      Error |     StdDev |    Gen 0 |   Gen 1 | Gen 2 |  Allocated |
|------------ |------------ |------------:|-----------:|-----------:|---------:|--------:|------:|-----------:|
| **OrleansPing** |           **1** |    **12.52 us** |  **0.1776 us** |  **0.1483 us** |   **0.0916** |       **-** |     **-** |    **1.72 KB** |
| **OrleansPing** |         **100** |   **644.22 us** |  **4.8800 us** |  **4.3260 us** |  **13.6719** |       **-** |     **-** |  **173.58 KB** |
| **OrleansPing** |        **1000** | **7,462.70 us** | **58.5400 us** | **51.8942 us** | **382.8125** | **62.5000** |     **-** | **1742.25 KB** |

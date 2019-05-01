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

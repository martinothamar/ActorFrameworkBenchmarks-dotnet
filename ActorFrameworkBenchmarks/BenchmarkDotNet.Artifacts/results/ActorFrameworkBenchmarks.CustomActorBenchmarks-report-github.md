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

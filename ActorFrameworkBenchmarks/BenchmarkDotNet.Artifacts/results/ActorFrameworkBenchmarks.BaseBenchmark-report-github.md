``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview4-011223
  [Host] : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|     Method | Count |          Mean |         Error |        StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|----------- |------ |--------------:|--------------:|--------------:|------:|------:|------:|----------:|
|   **BasePing** |     **1** |      **23.85 ns** |     **0.0593 ns** |     **0.0495 ns** |     **-** |     **-** |     **-** |         **-** |
| CustomPing |     1 |      70.46 ns |     0.3233 ns |     0.2866 ns |     - |     - |     - |         - |
|   **BasePing** | **10000** | **259,587.53 ns** |   **326.9019 ns** |   **272.9779 ns** |     **-** |     **-** |     **-** |         **-** |
| CustomPing | 10000 | 698,203.70 ns | 3,477.4531 ns | 3,082.6716 ns |     - |     - |     - |         - |

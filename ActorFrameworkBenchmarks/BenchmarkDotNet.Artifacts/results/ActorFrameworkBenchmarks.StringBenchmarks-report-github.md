``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-7700HQ CPU 2.80GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview4-011223
  [Host] : .NET Core 3.0.0-preview4-27615-11 (CoreCLR 4.6.27615.73, CoreFX 4.700.19.21213), 64bit RyuJIT

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
|                      Method |                  Test |      Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------------------- |---------------------- |----------:|----------:|----------:|------:|--------:|-------:|------:|------:|----------:|
|               **ToString_Base** |  **&lt;keep(...)9&quot; /&gt; [40]** |  **74.47 ns** | **0.6438 ns** | **0.6022 ns** |  **1.00** |    **0.00** | **0.0055** |     **-** |     **-** |     **104 B** |
|               ToString_Pool |  &lt;keep(...)9&quot; /&gt; [40] | 180.98 ns | 3.6413 ns | 5.8800 ns |  2.41 |    0.11 | 0.0055 |     - |     - |     104 B |
| ToString_ExistingBuffer_Ext |  &lt;keep(...)9&quot; /&gt; [40] |  86.15 ns | 0.4774 ns | 0.4232 ns |  1.16 |    0.01 | 0.0055 |     - |     - |     104 B |
|     ToString_ExistingBuffer |  &lt;keep(...)9&quot; /&gt; [40] |  88.40 ns | 0.3605 ns | 0.3195 ns |  1.19 |    0.01 | 0.0055 |     - |     - |     104 B |
|                             |                       |           |           |           |       |         |        |       |       |           |
|               **ToString_Base** | **&lt;matc(...)list&gt; [893]** | **772.05 ns** | **5.4475 ns** | **4.5489 ns** |  **1.00** |    **0.00** | **0.0963** |     **-** |     **-** |    **1808 B** |
|               ToString_Pool | &lt;matc(...)list&gt; [893] | 833.65 ns | 5.5852 ns | 4.6639 ns |  1.08 |    0.01 | 0.0963 |     - |     - |    1808 B |
| ToString_ExistingBuffer_Ext | &lt;matc(...)list&gt; [893] | 748.51 ns | 8.0769 ns | 6.3059 ns |  0.97 |    0.01 | 0.0963 |     - |     - |    1808 B |
|     ToString_ExistingBuffer | &lt;matc(...)list&gt; [893] | 745.23 ns | 5.2573 ns | 4.3901 ns |  0.97 |    0.01 | 0.0963 |     - |     - |    1808 B |

using BenchmarkDotNet.Configs;
using AprioriFromScratch.Benchmark.Columns;

namespace AprioriFromScratch.Benchmark.Config;

public class CustomBenchmarkConfig : ManualConfig
{
    public CustomBenchmarkConfig()
    {
        AddColumn(new AdditionalDetailsColumn());
    }
}
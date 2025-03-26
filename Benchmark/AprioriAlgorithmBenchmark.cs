using AprioriFromScratch.Implementations;
using AprioriFromScratch.Models;
using BenchmarkDotNet.Attributes;

namespace AprioriFromScratch.Benchmark;

public class AprioriAlgorithmBenchmark
{
    private AprioriAlgorithm _aprioriAlgorithm;
    private readonly IOFileService _ioService;
    private readonly Dictionary<string, HashSet<int>> _dataset;

    [Params(2, 3, 4)]
    public int MaxAntecedentSize;

    [Params(0.01, 0.05, 0.1)]
    public double RecurrenceSupportThreshold;

    [Params(0.01, 0.05, 0.1)]
    public double PairsRecurrenceSupportThreshold;

    [Params(0.2, 0.3, 0.5)]
    public double ConfidenceThreshold;

    [Params(0.2, 0.3, 0.5)]
    public double InterestThreshold;

    public AprioriAlgorithmBenchmark()
    {
        _aprioriAlgorithm = null!;

        _ioService = new IOFileService(
            _inputFilePath: "C:\\Users\\Andrei\\source\\repos\\AprioriFromScratch\\dataset.csv",
            _outputFilePath: "C:\\Users\\Andrei\\source\\repos\\AprioriFromScratch\\result.csv");

        _dataset = _ioService.LoadDataset()
                             .GetAwaiter()
                             .GetResult();
    }

    [GlobalSetup]
    public void Setup()
    {
        _aprioriAlgorithm = new AprioriAlgorithm(MaxAntecedentSize,
                                                 RecurrenceSupportThreshold,
                                                 PairsRecurrenceSupportThreshold,
                                                 ConfidenceThreshold,
                                                 InterestThreshold);
    }

    [Benchmark]
    public List<AssociationRule> TestGenerateAssociationRules()
    {
        return _aprioriAlgorithm.GenerateAssociationRules(_dataset);
    }
}
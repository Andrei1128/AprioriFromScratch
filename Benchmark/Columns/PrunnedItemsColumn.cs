using AprioriFromScratch.Implementations;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace AprioriFromScratch.Benchmark.Columns;

public class AdditionalDetailsColumn : IColumn
{
    public string Id => nameof(AdditionalDetailsColumn);
    public string ColumnName => "AdditionalDetails";
    public bool AlwaysShow => true;
    public ColumnCategory Category => ColumnCategory.Custom;
    public UnitType UnitType => UnitType.Dimensionless;
    public bool IsNumeric => true;
    public int PriorityInCategory => 0;
    public string Legend => $"Additional details from the benchmarked algorithm";

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;
    public bool IsAvailable(Summary summary) => true;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        int MaxAntecedentSize = (int)benchmarkCase.Parameters["MaxAntecedentSize"];
        double RecurrenceSupportThreshold = (double)benchmarkCase.Parameters["RecurrenceSupportThreshold"];
        double PairsRecurrenceSupportThreshold = (double)benchmarkCase.Parameters["PairsRecurrenceSupportThreshold"];
        double ConfidenceThreshold = (double)benchmarkCase.Parameters["ConfidenceThreshold"];
        double InterestThreshold = (double)benchmarkCase.Parameters["InterestThreshold"];

        var apriori = new AprioriAlgorithm(MaxAntecedentSize, RecurrenceSupportThreshold, PairsRecurrenceSupportThreshold, ConfidenceThreshold, InterestThreshold);

        var ioService = new IOFileService(
            _inputFilePath: "C:\\Users\\Andrei\\source\\repos\\AprioriFromScratch\\dataset.csv",
            _outputFilePath: "C:\\Users\\Andrei\\source\\repos\\AprioriFromScratch\\result.csv");

        var dataset = ioService.LoadDataset()
                               .GetAwaiter()
                               .GetResult();

        AprioriAlgorithmBenchmark.NeedsSetupDetails = true;

        apriori.GenerateAssociationRules(dataset);

        var concatedRulesCountPerTerms = apriori.RulesCountPerTerms.Select(x => $"{x.Key}: {x.Value}")
                                                                   .Aggregate((x1, x2) => $"{x1}, {x2}");

        return $"Baskets count: {apriori.BasketsCount}, Pruned items count: {apriori.PrunedItemsCount}, Rules count per terms: {concatedRulesCountPerTerms}";
    }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
        => GetValue(summary, benchmarkCase);
}
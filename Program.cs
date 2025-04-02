using AprioriFromScratch.Benchmark;
using AprioriFromScratch.Contracts;
using AprioriFromScratch.Implementations;
using BenchmarkDotNet.Running;

internal class Program
{
    private readonly IAprioriAlgorithm _aprioriAlgorithm;
    private readonly IIOService _ioService;

    private Program()
    {
        _aprioriAlgorithm = new AprioriAlgorithm(_maxAntecendentSize: 4,
                                                 _recurrenceSupportThreshold: 0.01,
                                                 _pairsRecurrenceSupportThreshold: 0.01,
                                                 _confidenceThreshold: 0.2,
                                                 _interestThreshold: 0.1);

        _ioService = new IOFileService(_inputFilePath: "C:\\Users\\Andrei\\source\\repos\\AprioriFromScratch\\dataset.csv",
                                       _outputFilePath: "C:\\Users\\Andrei\\source\\repos\\AprioriFromScratch\\result.csv");
    }

    private Program(IAprioriAlgorithm aprioriAlgorithm, IIOService ioService)
    {
        _aprioriAlgorithm = aprioriAlgorithm;
        _ioService = ioService;
    }

    private async Task RunAsync()
    {
        var items = await _ioService.LoadDataset();

        var associationRules = _aprioriAlgorithm.GenerateAssociationRules(items);

        await _ioService.SaveAssociationRules(associationRules);
    }

    public static async Task Main(string[] args)
    {
        if (args.Contains("--benchmark"))
        {
            BenchmarkRunner.Run<AprioriAlgorithmBenchmark>();
        }
        else
        {
            var program = new Program();
            await program.RunAsync();
        }
    }
}

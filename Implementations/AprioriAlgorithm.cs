using AprioriFromScratch.Benchmark;
using AprioriFromScratch.Contracts;
using AprioriFromScratch.Models;

namespace AprioriFromScratch.Implementations;

public class AprioriAlgorithm(int _maxAntecendentSize,
                              double _recurrenceSupportThreshold,
                              double _pairsRecurrenceSupportThreshold,
                              double _confidenceThreshold,
                              double _interestThreshold) : IAprioriAlgorithm
{
    public int BasketsCount { get; set; } = 0;
    public int PrunedItemsCount { get; set; } = 0;
    public Dictionary<int, int> RulesCountPerTerms { get; set; } = [];

    public List<AssociationRule> GenerateAssociationRules(Dictionary<string, HashSet<int>> items)
    {
        int basketsCount = items.Values.AsParallel()
                                       .SelectMany(set => set)
                                       .Distinct()
                                       .Count();

        var prunedItems = items.Where(x => CalculateSupport(x.Value.Count, basketsCount) >= _recurrenceSupportThreshold)
                               .ToDictionary(x => x.Key, x => x.Value);

        if (AprioriAlgorithmBenchmark.NeedsSetupDetails)
        {
            BasketsCount = basketsCount;
            PrunedItemsCount = prunedItems.Count;
        }

        if (prunedItems.Count == 0)
            return [];

        List<AssociationRule> associationRules = [];

        var antecedents = prunedItems.ToDictionary(x => new HashSet<string> { x.Key },
                                                    x => x.Value);

        for (int i = 2; i <= _maxAntecendentSize; i++)
        {
            Dictionary<HashSet<string>, HashSet<int>> newAntecedents = [];

            Parallel.ForEach(antecedents, (antecedent) =>
            {
                foreach (var (itemKey, itemBaskets) in prunedItems)
                {
                    if (antecedent.Key.Contains(itemKey))
                        continue;

                    var commonBaskets = GetCommonBaskets(antecedent.Value, itemBaskets);
                    if (commonBaskets.Count == 0)
                        continue;

                    double support = CalculateSupport(commonBaskets.Count, basketsCount);
                    if (support < _pairsRecurrenceSupportThreshold)
                        continue;

                    double confidence = CalculateConfidence(commonBaskets.Count, antecedent.Value.Count);
                    if (confidence <= _confidenceThreshold)
                        continue;

                    double interest = CalculateInterest(confidence, itemBaskets.Count, basketsCount);
                    if (interest <= _interestThreshold)
                        continue;

                    lock (associationRules)
                    {
                        associationRules.Add(new AssociationRule
                        {
                            Antecedent = antecedent.Key,
                            Consequent = itemKey,
                            Support = support,
                            Confidence = confidence,
                            Interest = interest
                        });
                    }

                    lock (newAntecedents)
                    {
                        newAntecedents[[.. antecedent.Key, itemKey]] = commonBaskets;
                    }
                }
            });

            antecedents = newAntecedents;

            if (AprioriAlgorithmBenchmark.NeedsSetupDetails)
            {
                RulesCountPerTerms.Add(i, antecedents.Count);
            }
        }

        return associationRules;
    }

    private static HashSet<int> GetCommonBaskets(HashSet<int> set1, HashSet<int> set2)
    {
        return set1.Count < set2.Count
            ? new HashSet<int>(set1.Where(set2.Contains))
            : new HashSet<int>(set2.Where(set1.Contains));
    }

    private static double CalculateSupport(double commonBaskets, double totalBaskets)
    {
        return commonBaskets / totalBaskets;
    }

    private static double CalculateConfidence(double commonBaskets, int antecedentBasketsCount)
    {
        return commonBaskets / antecedentBasketsCount;
    }

    private static double CalculateInterest(double confidence, double consequentBasketsCount, double totalBaskets)
    {
        return Math.Abs(confidence - consequentBasketsCount / totalBaskets);
    }

    public void SetParameters(int? maxAntecendentSize = null,
                              double? recurrenceSupportThreshold = null,
                              double? pairsRecurrenceSupportThreshold = null,
                              double? confidenceThreshold = null,
                              double? interestThreshold = null)
    {
        _maxAntecendentSize = maxAntecendentSize ?? _maxAntecendentSize;
        _recurrenceSupportThreshold = recurrenceSupportThreshold ?? _recurrenceSupportThreshold;
        _pairsRecurrenceSupportThreshold = pairsRecurrenceSupportThreshold ?? _pairsRecurrenceSupportThreshold;
        _confidenceThreshold = confidenceThreshold ?? _confidenceThreshold;
        _interestThreshold = interestThreshold ?? _interestThreshold;
    }
}

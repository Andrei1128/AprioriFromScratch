using AprioriFromScratch.Contracts;
using AprioriFromScratch.Models;

namespace AprioriFromScratch.Implementations;

public class AprioriAlgorithm(int _maxAntecendentSize,
                              double _recurrenceSupportThreshold,
                              double _pairsRecurrenceSupportThreshold,
                              double _confidenceThreshold,
                              double _interestThreshold) : IAprioriAlgorithm
{
    public List<AssociationRule> GenerateAssociationRules(Dictionary<string, HashSet<int>> items)
    {
        int basketsCount = items.Values.SelectMany(set => set)
                                       .Distinct()
                                       .Count();

        var prunnedItems = items.Where(x => CalculateSupport(x.Value.Count, basketsCount) > _recurrenceSupportThreshold)
                                .ToDictionary(x => x.Key, x => x.Value);

        List<AssociationRule> associationRules = [];

        if (prunnedItems.Count == 0)
        {
            return associationRules;
        }

        var antecedents = prunnedItems.ToDictionary(x => new HashSet<string> { x.Key },
                                                    x => x.Value);

        for (int i = 2; i <= _maxAntecendentSize; i++)
        {
            Dictionary<HashSet<string>, HashSet<int>> newAntecedents = [];

            foreach (var antecendent in antecedents)
            {
                foreach (var item in prunnedItems)
                {
                    if (antecendent.Key.Contains(item.Key))
                    {
                        continue;
                    }

                    var commonBaskets = antecendent.Value.Count < item.Value.Count
                                      ? antecendent.Value.Intersect(item.Value).ToHashSet()
                                      : item.Value.Intersect(antecendent.Value).ToHashSet();

                    if (commonBaskets.Count == 0)
                    {
                        continue;
                    }

                    double support = CalculateSupport(commonBaskets.Count, basketsCount);

                    if (support < _pairsRecurrenceSupportThreshold)
                    {
                        continue;
                    }

                    double confidence = CalculateConfidence(commonBaskets.Count, antecendent.Value.Count);

                    if (confidence > _confidenceThreshold)
                    {
                        double interest = CalculateInterest(confidence, item.Value.Count, basketsCount);

                        if (interest > _interestThreshold)
                        {
                            associationRules.Add(
                                new AssociationRule
                                {
                                    Antecedent = antecendent.Key,
                                    Consequent = item.Key,
                                    Support = support,
                                    Confidence = confidence,
                                    Interest = interest
                                });

                            newAntecedents[[.. antecendent.Key, item.Key]] = commonBaskets;
                        }
                    }
                }
            }

            antecedents = newAntecedents;
        }

        return associationRules;
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

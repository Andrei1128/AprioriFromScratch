using AprioriFromScratch.Models;

namespace AprioriFromScratch.Contracts;

public interface IAprioriAlgorithm
{
    List<AssociationRule> GenerateAssociationRules(Dictionary<string, HashSet<int>> items);
    void SetParameters(int? maxAntecendentSize = null,
                       double? recurrenceSupportThreshold = null,
                       double? pairsRecurrenceSupportThreshold = null,
                       double? confidenceThreshold = null,
                       double? interestThreshold = null);
}

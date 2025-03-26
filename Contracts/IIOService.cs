using AprioriFromScratch.Models;

namespace AprioriFromScratch.Contracts;

public interface IIOService
{
    Task<Dictionary<string, HashSet<int>>> LoadDataset();
    Task SaveAssociationRules(List<AssociationRule> associationRules);
}

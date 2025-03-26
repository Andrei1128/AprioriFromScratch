using AprioriFromScratch.Contracts;
using AprioriFromScratch.Models;
using System.Text;

namespace AprioriFromScratch.Implementations;

public class IOFileService(string _inputFilePath, string _outputFilePath) : IIOService
{
    public async Task<Dictionary<string, HashSet<int>>> LoadDataset()
    {
        string[] dataset = await File.ReadAllLinesAsync(_inputFilePath);
        Dictionary<string, HashSet<int>> items = [];

        foreach (string line in dataset)
        {
            string[] rawItems = line.Split(',');

            if (rawItems.Length < 2 || !int.TryParse(rawItems[0], out int basketNo))
            {
                Console.WriteLine($"Skipping invalid line: {line}");
                continue;
            }

            string itemName = rawItems[1];

            if (!items.TryGetValue(itemName, out var basketSet))
            {
                basketSet = [];
                items[itemName] = basketSet;
            }

            basketSet.Add(basketNo);
        }

        return items;
    }

    public async Task SaveAssociationRules(List<AssociationRule> associationRules)
    {
        var sb = new StringBuilder("antecedent,consequent,support,confidence,interest\n");

        foreach (var rule in associationRules.OrderBy(x => x.Interest))
        {
            sb.AppendLine($"{string.Join("|", rule.Antecedent)},{rule.Consequent},{rule.Support},{rule.Confidence},{rule.Interest}");
        }

        await File.WriteAllTextAsync(_outputFilePath, sb.ToString());
    }
}

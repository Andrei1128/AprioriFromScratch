namespace AprioriFromScratch.Models;

public class AssociationRule
{
    public HashSet<string> Antecedent { get; set; } = [];
    public string Consequent { get; set; } = string.Empty;
    public double Support { get; set; }
    public double Confidence { get; set; }
    public double Interest { get; set; }
}
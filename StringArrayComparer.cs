namespace AprioriFromScratch;

public class StringArrayComparer : IEqualityComparer<string[]>
{
    public bool Equals(string[]? x, string[]? y)
    {
        if (x == null || y == null) return false;
        if (x.Length != y.Length) return false;

        return x.OrderBy(s => s).SequenceEqual(y.OrderBy(s => s));
    }

    public int GetHashCode(string[] obj)
    {
        if (obj == null) return 0;
        unchecked
        {
            return obj.OrderBy(s => s)
                      .Aggregate(17, (hash, item) => hash * 31 + item.GetHashCode());
        }
    }
}
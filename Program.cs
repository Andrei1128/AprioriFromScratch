using AprioriFromScratch;
using System.Diagnostics;

var dataset = await File.ReadAllLinesAsync("C:\\Users\\Andrei\\source\\repos\\AprioriFromScratch\\l34.csv");

var sw = new Stopwatch();
sw.Start();

Dictionary<string, List<int>> items = [];

HashSet<int> baskets = [];

foreach (var line in dataset)
{
    var rawItems = line.Split(',');

    var basketNo = int.Parse(rawItems[0]);
    var itemName = rawItems[1];

    var inserted = items.TryAdd(itemName, [basketNo]);

    if (!inserted)
    {
        items[itemName].Add(basketNo);
    }

    baskets.Add(basketNo);
}

int basketsCount = baskets.Count;

var recurrenceSupportThreshold = 0.01; // parametrizable

Dictionary<string, List<int>> filteredItems = items.Where(x =>
{
    var itemSupport = (double)x.Value.Count / basketsCount;

    if (itemSupport > recurrenceSupportThreshold)
    {
        return true;
    }

    return false;

}).ToDictionary();

var lengthOfPairs = 2; // parametrizable
var pairsRecurrenceSupportThreshold = 0.01; // parametrizable
var confidenceThreshold = 0.2; // parametrizable
var liftThreshold = 1; // parametrizable

for (int i = 2; i <= lengthOfPairs; i++)
{
    var pairs = new HashSet<string[]>(new StringArrayComparer());
    // TODO: implement for more than 2 items
    foreach (var item1 in filteredItems)
    {
        foreach (var item2 in filteredItems)
        {
            if (item1.Key == item2.Key)
            {
                continue;
            }

            string[] pairItems = [item1.Key, item2.Key];

            if (!pairs.Add(pairItems))
            {
                continue;
            }

            var commonBaskets = item1.Value.Intersect(item2.Value)
                                           .ToList();

            if (commonBaskets.Count == 0)
            {
                continue;
            }

            var support = (double)commonBaskets.Count / basketsCount;

            if (support < pairsRecurrenceSupportThreshold)
            {
                continue;
            }

            var confidence = (double)commonBaskets.Count / item1.Value.Count;

            if (confidence > confidenceThreshold)
            {
                var lift = confidence / item2.Value.Count * basketsCount;

                if (lift > liftThreshold)
                {
                    Console.WriteLine($"Pair: {string.Join(", ", pairItems)}, with Support {support} and Confidence {confidence} and Lift {lift}");
                }
            }
        }
    }
}

sw.Stop();
Console.WriteLine($"Elapsed ms: {sw.ElapsedMilliseconds}");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
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

Dictionary<string, List<int>> filteredItems = [];

foreach (var item in items)
{
    var itemSupport = (double)item.Value.Count / basketsCount;

    if (itemSupport > recurrenceSupportThreshold)
    {
        filteredItems.Add(item.Key, item.Value);
    }
}

var lengthOfPairs = 2; // parametrizable
var pairsRecurrenceSupportThreshold = 0.01; // parametrizable
var confidenceThreshold = 0.2; // parametrizable

var pairs = new HashSet<string[]>(new StringArrayComparer());

for (int i = 2; i <= lengthOfPairs; i++)
{
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

            var aboveConfidence = true;

            foreach (var item in pairItems) // its ok?
            {
                var confidence = (double)commonBaskets.Count / filteredItems[item].Count;

                if (confidence < confidenceThreshold)
                {
                    aboveConfidence = false;
                    break;
                }

                if (aboveConfidence)
                {
                    Console.WriteLine($"Pair: {string.Join(", ", pairItems)}, with Support {support} and Confidence {confidence}");
                }

                break;
            }

            //if (aboveConfidence)
            //{
            //    Console.WriteLine($"Pair: {string.Join(", ", pairItems)}, with Support {support} and Confidence {0}");
            //}
        }
    }
}

sw.Stop();
Console.WriteLine($"Elapsed ms: {sw.ElapsedMilliseconds}");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
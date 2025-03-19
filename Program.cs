using System.Diagnostics;

var filePath = "C:\\Users\\Andrei\\source\\repos\\AprioriFromScratch\\l34.csv"; // parametrizable
var dataset = await File.ReadAllLinesAsync(filePath);

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

    return itemSupport > recurrenceSupportThreshold;

}).ToDictionary();

var lengthOfPairs = 3; // parametrizable
var pairsRecurrenceSupportThreshold = 0.01; // parametrizable
var confidenceThreshold = 0.2; // parametrizable
var interestThreshold = 0.1; // parametrizable

Dictionary<string[], List<int>> interestingPairs = [];

Dictionary<string[], List<int>> candidates = filteredItems.Select(x => new KeyValuePair<string[], List<int>>([x.Key], x.Value))
                                                          .ToDictionary();

for (int i = 2; i <= lengthOfPairs; i++)
{
    foreach (var candidate in candidates)
    {
        foreach (var item in filteredItems)
        {
            if (candidate.Key.Contains(item.Key))
            {
                continue;
            }

            var commonBaskets = candidate.Value.Intersect(item.Value)
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

            var confidence = (double)commonBaskets.Count / candidate.Value.Count;

            if (confidence > confidenceThreshold)
            {
                var interest = Math.Abs(confidence - (double)item.Value.Count / basketsCount);

                if (interest > interestThreshold)
                {
                    interestingPairs.Add([.. candidate.Key, item.Key], commonBaskets);
                    Console.WriteLine($"{string.Join("&&", candidate.Key)} => {item.Key}, with Support: {support}, Confidence: {confidence}, Interest: {interest}");
                }
            }
        }
    }

    candidates = interestingPairs.Where(x => x.Key.Length == i)
                                 .ToDictionary();
}

sw.Stop();
Console.WriteLine($"Elapsed ms: {sw.ElapsedMilliseconds}");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
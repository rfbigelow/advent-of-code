
internal static class P3
{
    private static int ValueToPriority(char value) => char.IsAsciiLetterLower(value)
        ? value - 'a' + 1
        : char.IsAsciiLetterUpper(value)
            ? value - 'A' + 27
            : throw new ArgumentOutOfRangeException(nameof(value), value, "Must be an ASCII character");

    private static char PriorityToValue(int priority) => priority switch
    {
        (>= 1) and (<= 26) => (char)((priority - 1) + 'a'),
        (>= 27) and (<= 52) => (char)((priority - 27) + 'A'),
        _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, "Must be in 1..26 or 27..52")
    };

    private static ulong CreatePrioritySet(IEnumerable<char> rucksack) => 
        rucksack
            .Select(x => ValueToPriority(x))
            .Aggregate((ulong)0, (agg, x) => agg | ((ulong)1 << x));

    internal static async Task<int> SumCommonPriorities(FileInfo file)
    {
        var rucksacks = File.ReadLinesAsync(file.FullName);
        var sum = 0;

        await foreach(var rucksack in rucksacks)
        {
            var mid = rucksack.Length / 2;
            var c1 = CreatePrioritySet(rucksack[..mid]);
            var c2 = CreatePrioritySet(rucksack[mid..]);
            var common = c1 & c2;
            System.Diagnostics.Debug.Assert(ulong.IsPow2(common));
            var priority = ulong.Log2(common);
            sum += (int)priority;
        }
        return sum;
    }

    internal static async Task<int> SumGroupPriorities(FileInfo file)
    {
        var rucksacks = File.ReadLinesAsync(file.FullName);
        var sum = 0;

        await foreach(var group in rucksacks.Buffer(3))
        {
            var first = CreatePrioritySet(group[0]);
            var second = CreatePrioritySet(group[1]);
            var third = CreatePrioritySet(group[2]);
            var common = first & second & third;
            Console.WriteLine($"{first} {second} {third} {common}");
            System.Diagnostics.Debug.Assert(ulong.IsPow2(common));
            var priority = ulong.Log2(common);
            sum += (int)priority;
        }
        return sum;
    }
}
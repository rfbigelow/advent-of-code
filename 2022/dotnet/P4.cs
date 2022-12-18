internal static class P4
{
    internal readonly struct ClosedRange
    {
        internal int Start {get;}
        internal int End {get;}

        internal ClosedRange(int start, int end)
        {
            if(end < start) throw new ArgumentException($"{nameof(start)} must be less than or equal to {nameof(end)}");

            Start = start;
            End = end;
        }

        internal bool Overlaps(ClosedRange other) => 
            (Start >= other.Start && Start <= other.End) || 
            (End >= other.Start && End <= other.End) ||
            (Start <= other.Start && End >= other.End);

        internal bool Contains(ClosedRange other) => Start <= other.Start && End >= other.End;

        internal static ClosedRange Parse(string input)
        {
            var parts = input.Split('-');
            var start = int.Parse(parts[0]);
            var end = int.Parse(parts[1]);
            return new ClosedRange(start, end);
        }
   }

    internal static async Task<int> SumFullContainments(FileInfo file) => await File.ReadLinesAsync(file.FullName)
        .Select(ParsePair)
        .Select(p => p.First.Contains(p.Second) || p.Second.Contains(p.First) ? 1 : 0)
        .SumAsync();

    internal static async Task<int> SumOverlappingAssignments(FileInfo file) => await File.ReadLinesAsync(file.FullName)
        .Select(ParsePair)
        .Select(p => p.First.Overlaps(p.Second) ? 1 : 0)
        .SumAsync();

    private static (ClosedRange First, ClosedRange Second) ParsePair(string input)
    {
        var parts = input.Split(',');
        return (ClosedRange.Parse(parts[0]), ClosedRange.Parse(parts[1]));
    }
}
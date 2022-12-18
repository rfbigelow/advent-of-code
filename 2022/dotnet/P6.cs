namespace aoc22;

internal static class P6
{
    private static int GetKey(char c) => c - 'a';

    private static bool IsMarkerDetected(IEnumerable<int> counts) => counts.Max() == 1;

    internal static async Task<int> GetMarkerEndPosition(FileInfo file, int length)
    {
        var input = await File.ReadAllTextAsync(file.FullName);
        var counts = new int[26];

        foreach(var c in input.Take(length))
        {
            counts[GetKey(c)] += 1;
        }

        if(IsMarkerDetected(counts)) return length;

        for(int i = length; i < input.Length; i++)
        {
            counts[GetKey(input[i - length])] -= 1;
            counts[GetKey(input[i])] += 1;
            if(IsMarkerDetected(counts)) return i + 1;
        }

        return -1;
    }
}
using System.Text;
using System.Text.RegularExpressions;

namespace aoc22;

internal static class P5
{
    internal enum Mover
    {
        CrateMover9000,
        CrateMover9001
    }
    private static readonly Regex CommandRegex = new Regex(@"move (\d+) from (\d+) to (\d+)", RegexOptions.Compiled | RegexOptions.Singleline);
    private record struct Command(int Count, int From, int To);

    internal static async Task<string> GetTopCrates(FileInfo file, Mover mover)
    {
        using var reader = new StreamReader(new FileStream(file.FullName, FileMode.Open, FileAccess.Read));

        var stacks = await PrepareStacks(reader);

        await ProcessCommands(reader, stacks, mover);

        var sb = new StringBuilder(stacks.Count);
        foreach(var stack in stacks.Where(x => x.Count > 0))
        {
            sb.Append(stack.Peek());
        }
        return sb.ToString();
    }

    private static IEnumerable<(char Contents, int StackKey)> GetBoxes(string input) => input
        .Select((x, i) => (x, i))
        .Where(p => p.x == '[')
        .Select(p => (input[p.i + 1], p.i + 1));

    private static IDictionary<int, int> GetStackLookup(string input) => input
        .Select((x, i) => (x, i))
        .Where(p => char.IsAsciiDigit(p.x))
        .Select((p, j) => new KeyValuePair<int, int>(p.i, j))
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    private static Command GetCommand(string input)
    {
        var match = CommandRegex.Match(input);
        var count = int.Parse(match.Groups[1].Value);
        var from = int.Parse(match.Groups[2].Value);
        var to = int.Parse(match.Groups[3].Value);
        return new Command(count, from, to);
    }

    private static void ProcessCommand9000(IReadOnlyList<Stack<char>> stacks, Command command)
    {
        for(var i = 0; i < command.Count; i++)
        {
            var box = stacks[command.From - 1].Pop();
            stacks[command.To - 1].Push(box);
        }
    }
    
    private static void ProcessCommand9001(IReadOnlyList<Stack<char>> stacks, Command command)
    {
        var temp = new Stack<char>(command.Count);
        for(var i = 0; i < command.Count; i++)
        {
            var box = stacks[command.From - 1].Pop();
            temp.Push(box);
        }

        foreach(var box in temp)
        {
            stacks[command.To - 1].Push(box);
        }
    }

    private static async Task ProcessCommands(TextReader reader, IReadOnlyList<Stack<char>> stacks, Mover mover)
    {
        var line = await ReadNextNonEmptyLineAsync(reader);
        while (line is not null && line.StartsWith('m'))
        {
            var command = GetCommand(line);
            switch (mover)
            {
                case Mover.CrateMover9000:
                    ProcessCommand9000(stacks, command);
                    break;
                case Mover.CrateMover9001:
                    ProcessCommand9001(stacks, command);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mover));
            }

            line = await reader.ReadLineAsync();
        }
    }

    private static async Task<string?> ReadNextNonEmptyLineAsync(TextReader reader)
    {
        var line = await reader.ReadLineAsync();
        while (line is not null && line == string.Empty)
        {
            line = await reader.ReadLineAsync();
        }

        return line;
    }

    private static async Task<List<Stack<char>>> PrepareStacks(TextReader reader)
    {
        var stacks = new List<Stack<char>>();
        var boxes = new Stack<IEnumerable<(char Contents, int StackKey)>>();

        var line = await reader.ReadLineAsync();
        while (line?.Contains('[') == true)
        {
            boxes.Push(GetBoxes(line));
            line = await reader.ReadLineAsync();
        }

        var stackLookup = GetStackLookup(line!);
        for (int i = 0; i < stackLookup.Count; i++)
        {
            stacks.Add(new Stack<char>());
        }

        while (boxes.Count > 0)
        {
            var row = boxes.Pop();
            foreach (var box in row)
            {
                var stackIndex = stackLookup[box.StackKey];
                stacks[stackIndex].Push(box.Contents);
            }
        }

        return stacks;
    }
}
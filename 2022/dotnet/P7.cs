using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace aoc22;

internal static class P7
{
    private interface INode
    {
        string Name { get; }
        long Size { get; }
    }

    private class Directory : INode, IEnumerable<Directory>
    {
        private readonly Dictionary<string, INode> _children = new();
        private readonly Lazy<long> _totalSize; 
        
        public string Name { get; }
        public long Size => _totalSize.Value;
        public Directory? Parent { get; }

        public Directory(string name, Directory? parent)
        {
            Name = name;
            Parent = parent;
            _totalSize = new Lazy<long>(() => _children.Select(x => x.Value.Size).Sum());
        }

        public void Add(INode node)
        {
            _ = _children.TryAdd(node.Name, node);
        }

        public Directory? GetChildDirectory(string name)
        {
            if (_children.TryGetValue(name, out var value))
            {
                return value as Directory;
            }

            return null;
        }

        public IEnumerator<Directory> GetEnumerator()
        {
            yield return this;
            foreach (var subDirectory in _children.Values.OfType<Directory>())
            {
                foreach (var directory in subDirectory)
                {
                    yield return directory;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private readonly struct File : INode
    {
        public string Name { get; }
        public long Size { get; }

        public File(string name, long size)
        {
            Name = name;
            Size = size;
        }
    }

    private static Func<string, Directory?, INode> CreateNodeBuilder()
    {
        var directoryRegex = new Regex(@"dir (\w+)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        var fileRegex = new Regex(@"(\d+) (\w+)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

        INode Parse(string input, Directory? currentDirectory)
        {
            var directoryMatch = directoryRegex.Match(input);
            if (directoryMatch.Success)
            {
                var directoryName = directoryMatch.Groups[1].Value;
                return new Directory(directoryName, currentDirectory);
            }

            var fileMatch = fileRegex.Match(input);
            if (!fileMatch.Success)
                throw new ArgumentException($"Could not parse node input \"{input}\"", nameof(input));
            
            var fileSize = long.Parse(fileMatch.Groups[1].Value);
            var fileName = fileMatch.Groups[2].Value;
            return new File(fileName, fileSize);
        }

        return Parse;
    }

    private static async Task ListDirectory(Directory directory, TextReader reader, Func<string, Directory?, INode> buildNode)
    {
        var line = await reader.ReadLineAsync();
        while (line != null)
        {
            var node = buildNode(line, directory);
            directory.Add(node);

            if (reader.Peek() == '$') break;

            line = await reader.ReadLineAsync();
        }
    }

    private static Directory ChangeDirectory(Directory? currentDirectory, string directoryName)
    {
        if (directoryName.Equals("..")) return currentDirectory!.Parent ?? throw new InvalidOperationException("Cannot get parent of root directory");
        
        var directory = currentDirectory?.GetChildDirectory(directoryName) ??
                        new Directory(directoryName, currentDirectory);
        return directory;
    }

    private static Func<Directory?, string, TextReader, Task<Directory>> CreateCommandProcessor()
    {
         var changeDirectoryCommandRegex = new Regex(@"\$ cd (/|\.\.|\w+)",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        var listDirectoryCommandRegex = new Regex(@"\$ ls",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);
        var nodeBuilder = CreateNodeBuilder();

        async Task<Directory> ProcessCommand(Directory? currentDirectory, string input, TextReader reader)
        {
            var listDirectoryCommandMatch = listDirectoryCommandRegex.Match(input);
            if (listDirectoryCommandMatch.Success)
            {
                await ListDirectory(
                    currentDirectory ?? throw new InvalidOperationException("cannot list a null directory"), reader,
                    nodeBuilder);
                return currentDirectory;
            }

            var changeDirectoryCommandMatch = changeDirectoryCommandRegex.Match(input);
            if (!changeDirectoryCommandMatch.Success)
                throw new InvalidOperationException($"Could not process command \"{input}\"");

            var directoryName = changeDirectoryCommandMatch.Groups[1].Value;
            return ChangeDirectory(currentDirectory, directoryName);
        }

        return ProcessCommand;
    }

    private static async Task<Directory> BuildFileSystem(TextReader reader)
    {
        Directory? currentDirectory = null;
        var processCommand = CreateCommandProcessor();
        var line = await reader.ReadLineAsync();

        while (!string.IsNullOrEmpty(line))
        {
            currentDirectory = await processCommand(currentDirectory, line, reader);
            line = await reader.ReadLineAsync();
        }

        while (currentDirectory?.Parent != null)
        {
            currentDirectory = currentDirectory.Parent;
        }

        Debug.Assert(currentDirectory != null, nameof(currentDirectory) + " != null");
        return currentDirectory;
    }

    internal static async Task<long> GetTotalSize(FileInfo file)
    {
        using var reader = new StreamReader(new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
        var currentDirectory = await BuildFileSystem(reader);

        return currentDirectory
            .Where(d => d.Size <= 100000)
            .Sum(d => d.Size);
    }

    internal static async Task<long> FindDirectorySize(FileInfo file, long capacity, long updateSize)
    {
        using var reader = new StreamReader(new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
        var currentDirectory = await BuildFileSystem(reader);

        var unused = capacity - currentDirectory.Size;
        var needed = updateSize - unused;

        var candidates = currentDirectory
            .Where(d => d.Size >= needed);

        return candidates
            .OrderBy(d => d.Size)
            .First().Size;
    }
}
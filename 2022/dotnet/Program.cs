using System.CommandLine;
using aoc22;

var rootCommand = new RootCommand("Advent of Code 2022 solver")
{
    // day 1
    CreateCommand<FileInfo, int>(
        "p1.1", 
        "Max calories carried by one elf", 
        P1.FindMaxCalories),
    CreateCommand<FileInfo, int>(
        "p1.2",
        "Calories carried by top 3 elves",
        P1.FindCaloriesForTop3),
    // day 2
    CreateCommand<FileInfo, int>(
        "p2.1",
        "Evaluate a rock, paper, scissors strategy guide...maybe",
        P2.EvaluateStrategyGivenMoves),
    CreateCommand<FileInfo, int>(
        "p2.2",
        "Evaluate a rock, paper, scissors strategy guide for realsies",
        P2.EvaluateStrategyGivenMoveAndOutcome),
    // day 3
    CreateCommand<FileInfo, int>(
        "p3.1",
        "Sum the item priorities for the item in both compartments for each rucksack",
        P3.SumCommonPriorities),
    CreateCommand<FileInfo, int>(
        "p3.2",
        "Sum the item priorities for each group's key",
        P3.SumGroupPriorities),
    // day 4
    CreateCommand<FileInfo, int>(
        "p4.1",
        "Sum the number of pairs where one range fully contains the other",
        P4.SumFullContainments
    ),
    CreateCommand<FileInfo, int>(
        "p4.2",
        "Sum the number of pairs where the assignments overlap",
        P4.SumOverlappingAssignments
    ),
    // day 5
    CreateCommand<FileInfo, string>(
        "p5.1",
        "Get the contents of the crate on the top of each stack",
        (file) => P5.GetTopCrates(file, P5.Mover.CrateMover9000)
    ),
    CreateCommand<FileInfo, string>(
        "p5.2",
        "Get the contents of the crate on the top of each stack using the CrateMover9001",
        (file) => P5.GetTopCrates(file, P5.Mover.CrateMover9001)
    ),
    // day 6
    CreateCommand<FileInfo, int>(
        "p6.1",
        "Find the position of the end of the start-of-packet marker",
        (file) => P6.GetMarkerEndPosition(file, 4)
    ),
    CreateCommand<FileInfo, int>(
        "p6.2",
        "Find the position of the end of the start-of-message marker",
        (file) => P6.GetMarkerEndPosition(file, 14)
    ),
    // day 7
    CreateCommand<FileInfo, long>(
        "p7.1",
        "Calculate size",
        P7.GetTotalSize),
    CreateCommand<FileInfo, long>(
        "p7.2",
        "Find directory size",
        (file) => P7.FindDirectorySize(file, 70000000, 30000000))
};

await rootCommand.InvokeAsync(args);

Command CreateCommand<T1, T2>(string name, string description, Func<T1, Task<T2>> handler)
{
    var command = new Command(name, description);
    var arg = new Argument<T1>();
    command.Add(arg);
    command.SetHandler(async x => Console.WriteLine($"{await handler(x)}"), arg);
    return command;
}
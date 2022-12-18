namespace aoc22;

internal static class P1
{
    public static async Task<int> FindMaxCalories(FileInfo file)
    {
        var lines = File.ReadLinesAsync(file.FullName);

        var calories = 0;
        var maxCalories = calories;

        await foreach(var line in lines)
        {
            if(string.IsNullOrEmpty(line))
            {
                maxCalories = Math.Max(calories, maxCalories);
                calories = 0;
                continue;
            }
            calories = calories + int.Parse(line);
        }

        maxCalories = Math.Max(calories, maxCalories);
        return maxCalories;
    }

    public static async Task<int> FindCaloriesForTop3(FileInfo file)
    {
        var lines = File.ReadLinesAsync(file.FullName);

        var calories = 0;
        var first = calories;
        var second = calories;
        var third = calories;

        await foreach(var line in lines)
        {
            if(string.IsNullOrEmpty(line))
            {
                (first, second, third) = Top3(calories, first, second, third);
                calories = 0;
                continue;
            }

            calories = calories + int.Parse(line);
        }

        (first, second, third) = Top3(calories, first, second, third);
        return first + second + third;
    }

    static (int First, int Second, int Third) Top3(int x, int first, int second, int third)
    {
        if(x >= first)
        {
            third = second;
            second = first;
            first = x;
        }
        else if(x >= second)
        {
            third = second;
            second = x;
        }
        else if(x > third)
        {
            third = x;
        }
        return (first, second, third);
    }
}
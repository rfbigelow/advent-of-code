internal static class P2
{
    enum Move
    {
        Rock,
        Paper,
        Scissors
    };

    enum Outcome
    {
        Loss,
        Draw,
        Win
    };

    internal static async Task<int> EvaluateStrategyGivenMoves(FileInfo file)
    {
        var lines = File.ReadLinesAsync(file.FullName);
        var totalScore = 0;

        await foreach(var line in lines)
        {
            var round = ParseMoves(line);
            totalScore += Evaluate(round.Opponent, round.Player);
        }
        return totalScore;
    }

    internal static async Task<int> EvaluateStrategyGivenMoveAndOutcome(FileInfo file)
    {
        var lines = File.ReadLinesAsync(file.FullName);
        var totalScore = 0;

        await foreach(var line in lines)
        {
            var round = ParseMoveAndOutcome(line);
            var player = ChoosePlayerMove(round.Opponent, round.Outcome);
            totalScore += Evaluate(round.Opponent, player);
        }
        return totalScore;
    }

    static (Move Opponent, Move Player) ParseMoves(string input)
    {
        var moves = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return (ParseOpponent(moves[0]), ParsePlayer(moves[1]));
    }

    static (Move Opponent, Outcome Outcome) ParseMoveAndOutcome(string input)
    {
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return (ParseOpponent(parts[0]), ParseOutcome(parts[1]));
    }

    static Move ParseOpponent(string input) => input switch
    {
        "A" => Move.Rock,
        "B" => Move.Paper,
        "C" => Move.Scissors,
        _ => throw new InvalidOperationException()
    };

    static Move ParsePlayer(string input) => input switch
    {
        "X" => Move.Rock,
        "Y" => Move.Paper,
        "Z" => Move.Scissors,
        _ => throw new InvalidOperationException()
    };

    static Outcome ParseOutcome(string input) => input switch
    {
        "X" => Outcome.Loss,
        "Y" => Outcome.Draw,
        "Z" => Outcome.Win,
        _ => throw new InvalidOperationException()
    };

    static Move ChoosePlayerMove(Move opponent, Outcome outcome)
    {
        switch(outcome)
        {
            case Outcome.Loss: return opponent switch
            {
                Move.Rock => Move.Scissors,
                Move.Paper => Move.Rock,
                Move.Scissors => Move.Paper,
                _ => throw new InvalidOperationException()
            };
            case Outcome.Draw: return opponent switch
            {
                _ => opponent
            };
            case Outcome.Win: return opponent switch
            {
                Move.Rock => Move.Paper,
                Move.Paper => Move.Scissors,
                Move.Scissors => Move.Rock,
                _ => throw new InvalidOperationException()
            };
        }
        throw new InvalidOperationException();
    }

    static int Evaluate(Move opponent, Move player)
    {
        return Score(opponent, player) + Score(player);
    }

    static int Score(Move opponent, Move player)
    {
        switch(player)
        {
            case Move.Rock: return opponent switch
            {
                Move.Scissors => 6,
                Move.Rock => 3,
                Move.Paper => 0,
                _ => throw new InvalidOperationException()
            };
            case Move.Paper: return opponent switch
            {
                Move.Rock => 6,
                Move.Paper => 3,
                Move.Scissors => 0,
                _ => throw new InvalidOperationException()
            };
            case Move.Scissors: return opponent switch
            {
                Move.Paper => 6,
                Move.Scissors => 3,
                Move.Rock => 0,
                _ => throw new InvalidOperationException()
            };
        }
        throw new InvalidOperationException();
    }

    static int Score(Move x) => x switch
    {
        Move.Rock => 1,
        Move.Paper => 2,
        Move.Scissors => 3,
        _ => throw new InvalidOperationException()
    };
}
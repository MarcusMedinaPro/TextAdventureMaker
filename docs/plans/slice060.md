## Slice 60: Puzzle Toolkit

**Mål:** Återanvändbara pusselmönster som kan konfigureras och kombineras.

**Referens:** `docs/plans/imported/Future_Development_Ideas.md`

### Task 60.1: IPuzzle interface

```csharp
public interface IPuzzle
{
    string Id { get; }
    string Name { get; }
    PuzzleState State { get; }
    bool IsSolved { get; }

    PuzzleResult Attempt(string input, IGameState gameState);
    string GetHint(int hintLevel);
    void Reset();
}

public enum PuzzleState
{
    Locked,      // Inte tillgängligt än
    Active,      // Kan försöka lösa
    Solved,      // Redan löst
    Failed       // Misslyckades permanent
}

public record PuzzleResult(bool Success, string Message, IEnumerable<GameEffect>? Effects = null);
```

### Task 60.2: Combination Lock Puzzle

```csharp
public class CombinationLockPuzzle : IPuzzle
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "Combination Lock";
    public PuzzleState State { get; private set; } = PuzzleState.Active;
    public bool IsSolved => State == PuzzleState.Solved;

    private readonly string _correctCombination;
    private readonly int _maxAttempts;
    private int _attempts;

    public CombinationLockPuzzle(string combination, int maxAttempts = -1)
    {
        _correctCombination = combination;
        _maxAttempts = maxAttempts;
    }

    public PuzzleResult Attempt(string input, IGameState gameState)
    {
        if (State != PuzzleState.Active)
            return new PuzzleResult(false, "The lock is no longer active.");

        _attempts++;

        if (input == _correctCombination)
        {
            State = PuzzleState.Solved;
            return new PuzzleResult(true, "Click! The lock opens.", [new UnlockEffect(Id)]);
        }

        if (_maxAttempts > 0 && _attempts >= _maxAttempts)
        {
            State = PuzzleState.Failed;
            return new PuzzleResult(false, "The lock mechanism jams permanently.");
        }

        // Ge feedback på hur nära man är
        var correctDigits = CountCorrectDigits(input);
        return new PuzzleResult(false, $"Wrong combination. {correctDigits} digit(s) in correct position.");
    }

    public string GetHint(int hintLevel) => hintLevel switch
    {
        1 => $"The combination has {_correctCombination.Length} digits.",
        2 => $"The first digit is {_correctCombination[0]}.",
        3 => $"The combination is {_correctCombination[..^1]}X.",
        _ => "No more hints available."
    };

    private int CountCorrectDigits(string input)
    {
        var count = 0;
        for (int i = 0; i < Math.Min(input.Length, _correctCombination.Length); i++)
        {
            if (input[i] == _correctCombination[i])
                count++;
        }
        return count;
    }

    public void Reset()
    {
        State = PuzzleState.Active;
        _attempts = 0;
    }
}
```

### Task 60.3: Sequence Puzzle (Simon Says)

```csharp
public class SequencePuzzle : IPuzzle
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "Sequence Puzzle";
    public PuzzleState State { get; private set; } = PuzzleState.Active;
    public bool IsSolved => State == PuzzleState.Solved;

    private readonly List<string> _correctSequence;
    private readonly List<string> _playerSequence = [];

    public SequencePuzzle(params string[] sequence)
    {
        _correctSequence = sequence.ToList();
    }

    public PuzzleResult Attempt(string input, IGameState gameState)
    {
        if (State != PuzzleState.Active)
            return new PuzzleResult(false, "The puzzle is no longer active.");

        _playerSequence.Add(input.ToLower());

        // Kolla om sekvensen är fel
        var index = _playerSequence.Count - 1;
        if (_correctSequence[index].ToLower() != input.ToLower())
        {
            _playerSequence.Clear();
            return new PuzzleResult(false, "Wrong! The sequence resets.");
        }

        // Kolla om sekvensen är komplett
        if (_playerSequence.Count == _correctSequence.Count)
        {
            State = PuzzleState.Solved;
            return new PuzzleResult(true, "The sequence is complete!");
        }

        return new PuzzleResult(false, $"Correct! {_correctSequence.Count - _playerSequence.Count} more to go.");
    }

    public string GetHint(int hintLevel) => hintLevel switch
    {
        1 => $"The sequence has {_correctSequence.Count} steps.",
        2 => $"It starts with '{_correctSequence[0]}'.",
        3 => $"The sequence is: {string.Join(", ", _correctSequence.Take(_correctSequence.Count - 1))}...",
        _ => "No more hints available."
    };

    public void Reset()
    {
        State = PuzzleState.Active;
        _playerSequence.Clear();
    }
}
```

### Task 60.4: Riddle Puzzle

```csharp
public class RiddlePuzzle : IPuzzle
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "Riddle";
    public PuzzleState State { get; private set; } = PuzzleState.Active;
    public bool IsSolved => State == PuzzleState.Solved;

    private readonly string _riddle;
    private readonly HashSet<string> _acceptedAnswers;

    public RiddlePuzzle(string riddle, params string[] answers)
    {
        _riddle = riddle;
        _acceptedAnswers = answers.Select(a => a.ToLower()).ToHashSet();
    }

    public string GetRiddle() => _riddle;

    public PuzzleResult Attempt(string input, IGameState gameState)
    {
        if (State != PuzzleState.Active)
            return new PuzzleResult(false, "The riddle has already been answered.");

        if (_acceptedAnswers.Contains(input.ToLower().Trim()))
        {
            State = PuzzleState.Solved;
            return new PuzzleResult(true, "Correct! The answer echoes through the chamber.");
        }

        return new PuzzleResult(false, "That is not the answer...");
    }

    public string GetHint(int hintLevel)
    {
        var answer = _acceptedAnswers.First();
        return hintLevel switch
        {
            1 => $"The answer has {answer.Length} letters.",
            2 => $"It starts with '{answer[0]}'.",
            3 => $"The answer is '{answer[..^2]}__'.",
            _ => "No more hints available."
        };
    }

    public void Reset() => State = PuzzleState.Active;
}

// Exempel:
// new RiddlePuzzle("What has keys but no locks?", "piano", "keyboard")
```

### Task 60.5: Multi-Step Puzzle

```csharp
public class MultiStepPuzzle : IPuzzle
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "Multi-Step Puzzle";
    public PuzzleState State { get; private set; } = PuzzleState.Active;
    public bool IsSolved => _currentStep >= _steps.Count;

    private readonly List<IPuzzle> _steps;
    private int _currentStep = 0;

    public MultiStepPuzzle(params IPuzzle[] steps)
    {
        _steps = steps.ToList();
    }

    public IPuzzle CurrentStep => _steps[_currentStep];

    public PuzzleResult Attempt(string input, IGameState gameState)
    {
        if (IsSolved)
            return new PuzzleResult(false, "Already solved.");

        var result = _steps[_currentStep].Attempt(input, gameState);

        if (result.Success)
        {
            _currentStep++;

            if (IsSolved)
            {
                State = PuzzleState.Solved;
                return new PuzzleResult(true, "You've completed all steps!");
            }

            return new PuzzleResult(true, $"Step complete! {_steps.Count - _currentStep} remaining.");
        }

        return result;
    }

    public string GetHint(int hintLevel) =>
        _steps[_currentStep].GetHint(hintLevel);

    public void Reset()
    {
        _currentStep = 0;
        State = PuzzleState.Active;
        foreach (var step in _steps)
            step.Reset();
    }
}
```

### Task 60.6: Environmental Puzzle

```csharp
public class EnvironmentalPuzzle : IPuzzle
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "Environmental Puzzle";
    public PuzzleState State { get; private set; } = PuzzleState.Active;
    public bool IsSolved => State == PuzzleState.Solved;

    private readonly Dictionary<string, bool> _conditions;
    private readonly Func<IGameState, bool> _solveCondition;

    public EnvironmentalPuzzle(Func<IGameState, bool> solveCondition)
    {
        _solveCondition = solveCondition;
        _conditions = [];
    }

    public void AddCondition(string id, bool required) => _conditions[id] = required;

    public PuzzleResult Attempt(string input, IGameState gameState)
    {
        // Environmental puzzles löses genom världsförändringar, inte direkt input
        return new PuzzleResult(false, "This puzzle requires changing the environment.");
    }

    public void CheckSolved(IGameState gameState)
    {
        if (_solveCondition(gameState))
        {
            State = PuzzleState.Solved;
        }
    }

    public string GetHint(int hintLevel) => hintLevel switch
    {
        1 => "Look around carefully. Something in the room might help.",
        2 => "Try interacting with objects in the environment.",
        _ => "No more hints available."
    };

    public void Reset() => State = PuzzleState.Active;
}

// Exempel: Spegelpussel där rätt objekt måste reflektera ljus
// new EnvironmentalPuzzle(state =>
//     state.HasFlag("mirror_positioned") &&
//     state.HasFlag("light_on") &&
//     state.CurrentLocation.Id == "sun_chamber")
```

### Task 60.7: PuzzleCommand

```csharp
public class SolveCommand(string puzzleId, string answer) : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var puzzle = context.State.GetPuzzle(puzzleId);
        if (puzzle == null)
            return CommandResult.Fail("There's no puzzle to solve here.");

        var result = puzzle.Attempt(answer, context.State);

        if (result.Effects != null)
        {
            foreach (var effect in result.Effects)
                effect.Apply(context.State);
        }

        return result.Success
            ? CommandResult.Ok(result.Message)
            : CommandResult.Fail(result.Message);
    }
}
```

### Task 60.8: Tester

```csharp
[Fact]
public void CombinationLock_SolvesWithCorrectCode()
{
    var puzzle = new CombinationLockPuzzle("1234");

    var result = puzzle.Attempt("1234", CreateState());

    Assert.True(result.Success);
    Assert.True(puzzle.IsSolved);
}

[Fact]
public void SequencePuzzle_RequiresCorrectOrder()
{
    var puzzle = new SequencePuzzle("red", "blue", "green");

    puzzle.Attempt("red", CreateState());
    puzzle.Attempt("green", CreateState());  // Fel ordning

    Assert.False(puzzle.IsSolved);
}
```

### Task 60.9: Sandbox — tempel med pussel

Demo med kombinationslås, gåtor och miljöpussel som måste lösas för att nå skatten.

---

## Completion Checklist
- [x] Core implementation for this slice is present in the engine.
- [x] Behaviour is covered by tests and/or deterministic validation paths.
- [x] Demo document: `60_The_Puzzle_Basement.md`.
- [x] Marked complete in project slice status.

// <copyright file="PuzzleTypes.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Combination lock puzzle - requires correct code entry.
/// </summary>
public sealed class CombinationLockPuzzle : IPuzzle
{
    private readonly string _correctCombination;
    private readonly int _maxAttempts;
    private int _attempts;

    public string Id { get; init; } = "";
    public string Name { get; init; } = "Combination Lock";
    public PuzzleState State { get; private set; } = PuzzleState.Active;
    public bool IsSolved => State == PuzzleState.Solved;

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
            return new PuzzleResult(true, "Click! The lock opens.");
        }

        if (_maxAttempts > 0 && _attempts >= _maxAttempts)
        {
            State = PuzzleState.Failed;
            return new PuzzleResult(false, "The lock mechanism jams permanently.");
        }

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

    public void Reset()
    {
        State = PuzzleState.Active;
        _attempts = 0;
    }

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
}

/// <summary>
/// Sequence puzzle - requires correct order of inputs (Simon Says style).
/// </summary>
public sealed class SequencePuzzle : IPuzzle
{
    private readonly List<string> _correctSequence;
    private readonly List<string> _playerSequence = [];

    public string Id { get; init; } = "";
    public string Name { get; init; } = "Sequence Puzzle";
    public PuzzleState State { get; private set; } = PuzzleState.Active;
    public bool IsSolved => State == PuzzleState.Solved;

    public SequencePuzzle(params string[] sequence)
    {
        _correctSequence = sequence.ToList();
    }

    public PuzzleResult Attempt(string input, IGameState gameState)
    {
        if (State != PuzzleState.Active)
            return new PuzzleResult(false, "The puzzle is no longer active.");

        _playerSequence.Add(input.ToLowerInvariant());

        if (_playerSequence.Count > _correctSequence.Count ||
            _correctSequence[_playerSequence.Count - 1].ToLowerInvariant() != input.ToLowerInvariant())
        {
            _playerSequence.Clear();
            return new PuzzleResult(false, "Wrong! The sequence resets.");
        }

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

/// <summary>
/// Riddle puzzle - requires correct answer input.
/// </summary>
public sealed class RiddlePuzzle : IPuzzle
{
    private readonly string _riddle;
    private readonly HashSet<string> _acceptedAnswers;

    public string Id { get; init; } = "";
    public string Name { get; init; } = "Riddle";
    public PuzzleState State { get; private set; } = PuzzleState.Active;
    public bool IsSolved => State == PuzzleState.Solved;

    public RiddlePuzzle(string riddle, params string[] answers)
    {
        _riddle = riddle;
        _acceptedAnswers = answers.Select(a => a.ToLowerInvariant()).ToHashSet();
    }

    public string GetRiddle() => _riddle;

    public PuzzleResult Attempt(string input, IGameState gameState)
    {
        if (State != PuzzleState.Active)
            return new PuzzleResult(false, "The riddle has already been answered.");

        if (_acceptedAnswers.Contains(input.ToLowerInvariant().Trim()))
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

/// <summary>
/// Multi-step puzzle - combines multiple puzzles into sequence.
/// </summary>
public sealed class MultiStepPuzzle : IPuzzle
{
    private readonly List<IPuzzle> _steps;
    private int _currentStep;

    public string Id { get; init; } = "";
    public string Name { get; init; } = "Multi-Step Puzzle";
    public PuzzleState State { get; private set; } = PuzzleState.Active;
    public bool IsSolved => _currentStep >= _steps.Count;
    public IPuzzle CurrentStep => _steps[_currentStep];

    public MultiStepPuzzle(params IPuzzle[] steps)
    {
        _steps = steps.ToList();
        _currentStep = 0;
    }

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

/// <summary>
/// Environmental puzzle - solved by changing world state rather than direct input.
/// </summary>
public sealed class EnvironmentalPuzzle : IPuzzle
{
    private readonly Func<IGameState, bool> _solveCondition;

    public string Id { get; init; } = "";
    public string Name { get; init; } = "Environmental Puzzle";
    public PuzzleState State { get; private set; } = PuzzleState.Active;
    public bool IsSolved => State == PuzzleState.Solved;

    public EnvironmentalPuzzle(Func<IGameState, bool> solveCondition)
    {
        _solveCondition = solveCondition;
    }

    public PuzzleResult Attempt(string input, IGameState gameState)
    {
        return new PuzzleResult(false, "This puzzle requires changing the environment.");
    }

    public void CheckSolved(IGameState gameState)
    {
        if (_solveCondition(gameState))
            State = PuzzleState.Solved;
    }

    public string GetHint(int hintLevel) => hintLevel switch
    {
        1 => "Look around carefully. Something in the room might help.",
        2 => "Try interacting with objects in the environment.",
        _ => "No more hints available."
    };

    public void Reset() => State = PuzzleState.Active;
}

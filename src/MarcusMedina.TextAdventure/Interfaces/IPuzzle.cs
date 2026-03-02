// <copyright file="IPuzzle.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Enum for puzzle states.
/// </summary>
public enum PuzzleState
{
    Locked,      // Not yet available
    Active,      // Can attempt to solve
    Solved,      // Already solved
    Failed       // Failed permanently
}

/// <summary>
/// Result of a puzzle attempt.
/// </summary>
public sealed record PuzzleResult(bool Success, string Message);

/// <summary>
/// Interface for reusable puzzle systems.
/// </summary>
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

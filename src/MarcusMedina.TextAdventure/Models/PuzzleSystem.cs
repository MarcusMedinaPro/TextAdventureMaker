// <copyright file="PuzzleSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// System for managing puzzle registry and state.
/// </summary>
public sealed class PuzzleSystem
{
    private readonly Dictionary<string, IPuzzle> _puzzles = [];

    public void RegisterPuzzle(string id, IPuzzle puzzle) => _puzzles[id] = puzzle;
    public IPuzzle? GetPuzzle(string id) => _puzzles.TryGetValue(id, out var p) ? p : null;
    public IEnumerable<IPuzzle> GetAllPuzzles() => _puzzles.Values;
    public IEnumerable<IPuzzle> GetActivePuzzles() => _puzzles.Values.Where(p => p.State == PuzzleState.Active);
    public IEnumerable<IPuzzle> GetSolvedPuzzles() => _puzzles.Values.Where(p => p.IsSolved);
}

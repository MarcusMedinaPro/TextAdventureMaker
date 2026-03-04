// <copyright file="PuzzleExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Extensions;

/// <summary>
/// Extension methods for puzzle system management.
/// </summary>
public static class PuzzleExtensions
{
    public static ILocation SetPuzzleSystem(this ILocation location, PuzzleSystem puzzleSystem)
    {
        location.PuzzleSystem = puzzleSystem;
        return location;
    }

    public static PuzzleSystem? GetPuzzleSystem(this ILocation location) =>
        location.PuzzleSystem;
}

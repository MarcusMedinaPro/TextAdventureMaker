// <copyright file="SolveCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Commands;

/// <summary>
/// Command to solve a puzzle.
/// </summary>
public sealed record SolveCommand(string PuzzleId, string Answer) : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var puzzleSystem = context.State.CurrentLocation.GetPuzzleSystem();
        if (puzzleSystem is null)
            return CommandResult.Fail("No puzzle system is active.", GameError.InvalidState);

        var puzzle = puzzleSystem.GetPuzzle(PuzzleId);
        if (puzzle is null)
            return CommandResult.Fail("There's no puzzle to solve here.", GameError.InvalidState);

        var result = puzzle.Attempt(Answer, context.State);

        return result.Success
            ? CommandResult.Ok(result.Message)
            : CommandResult.Fail(result.Message, GameError.InvalidState);
    }
}

// <copyright file="HistoryCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

/// <summary>
/// Displays a summary of the player's journey so far.
/// </summary>
public sealed class HistoryCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var generator = new StorySummaryGenerator();
        var summary = generator.Generate(context.State.PlayerHistory);
        return CommandResult.Ok(summary);
    }
}

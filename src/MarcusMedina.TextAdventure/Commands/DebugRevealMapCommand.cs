// <copyright file="DebugRevealMapCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class DebugRevealMapCommand(GameState state, string[] tokens) : ICommand
{
    public string Name => "revealmap";
    public string[]? Aliases => null;
    public string Description => "Reveal all locations (debug only)";

    public CommandResult Execute(CommandContext context)
    {
        if (!state.DebugMode)
            return CommandResult.Fail("Debug mode is not enabled.", GameError.None);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== World Map ===");

        foreach (var location in state.Locations)
        {
            string locName = location is Models.Location loc ? loc.Name : location.Id;
            sb.AppendLine($"\n{locName} ({location.Id})");
            if (location.Exits.Count > 0)
                sb.AppendLine($"  Exits: {string.Join(", ", location.Exits.Keys)}");
        }

        return CommandResult.Ok(sb.ToString());
    }
}

// <copyright file="DebugTeleportCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class DebugTeleportCommand(GameState state, string[] tokens) : ICommand
{
    public string Name => "teleport";
    public string[]? Aliases => ["tp"];
    public string Description => "Teleport to a location (debug only)";

    public CommandResult Execute(CommandContext context)
    {
        if (!state.DebugMode)
            return CommandResult.Fail("Debug mode is not enabled.", GameError.None);

        if (tokens.Length < 2)
            return CommandResult.Fail("Usage: teleport <location_id>", GameError.None);

        string locationId = string.Join(" ", tokens.Skip(1));
        var location = state.Locations.FirstOrDefault(l => l.Id == locationId);

        if (location  is null)
            return CommandResult.Fail($"Location '{locationId}' not found.", GameError.None);

        state.Teleport(location);
        string locName = location is Models.Location loc ? loc.Name : location.Id;
        return CommandResult.Ok($"Teleported to {locName}.");
    }
}

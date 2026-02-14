// <copyright file="DebugSetFlagCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class DebugSetFlagCommand(GameState state, string[] tokens) : ICommand
{
    public string Name => "setflag";
    public string[]? Aliases => null;
    public string Description => "Set a world flag (debug only)";

    public CommandResult Execute(CommandContext context)
    {
        if (!state.DebugMode)
            return CommandResult.Fail("Debug mode is not enabled.", GameError.None);

        if (tokens.Length < 3)
            return CommandResult.Fail("Usage: setflag <flag_name> <true|false>", GameError.None);

        string flagName = tokens[1];
        string value = tokens[2].ToLowerInvariant();

        if (!bool.TryParse(value, out bool flagValue))
            return CommandResult.Fail("Value must be 'true' or 'false'.", GameError.None);

        if (state.WorldState is WorldState worldState)
        {
            worldState.SetFlag(flagName, flagValue);
            return CommandResult.Ok($"Set flag '{flagName}' to {flagValue}.");
        }

        return CommandResult.Fail("WorldState not available.", GameError.None);
    }
}

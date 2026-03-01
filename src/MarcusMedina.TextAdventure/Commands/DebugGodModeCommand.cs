// <copyright file="DebugGodModeCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Commands;

public class DebugGodModeCommand(GameState state, string[] tokens) : ICommand
{
    public string Name => "godmode";
    public string[]? Aliases => null;
    public string Description => "Toggle god mode (debug only)";

    public CommandResult Execute(CommandContext context)
    {
        if (!state.DebugMode)
            return CommandResult.Fail("Debug mode is not enabled.", GameError.None);

        if (state.Stats.MaxHealth < 9000)
        {
            state.Stats.SetMaxHealth(9999);
            state.Stats.SetHealth(9999);
            return CommandResult.Ok("God mode activated. You are invincible.");
        }

        state.Stats.SetMaxHealth(100);
        state.Stats.SetHealth(100);
        return CommandResult.Ok("God mode deactivated.");
    }
}

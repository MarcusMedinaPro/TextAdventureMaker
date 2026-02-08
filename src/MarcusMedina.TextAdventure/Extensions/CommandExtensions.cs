// <copyright file="CommandExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Extensions;

public static class CommandExtensions
{
    /// <summary>
    /// Executes a command against the provided game state.
    /// </summary>
    public static CommandResult Execute(this GameState state, ICommand command)
    {
        CommandResult result = command.Execute(new CommandContext(state));
        if (!result.ShouldQuit)
        {
            state.TimeSystem.Tick(state);
            state.RandomEvents.Tick(state);
            state.TickNpcTriggers();
        }

        return result;
    }

    /// <summary>
    /// Executes the built-in LookCommand for the current state.
    /// </summary>
    public static CommandResult Look(this GameState state)
    {
        return state.Execute(new LookCommand());
    }

    /// <summary>
    /// Executes the built-in InventoryCommand for the current state.
    /// </summary>
    public static CommandResult InventoryView(this GameState state)
    {
        return state.Execute(new InventoryCommand());
    }

    /// <summary>
    /// Executes the built-in StatsCommand for the current state.
    /// </summary>
    public static CommandResult StatsView(this GameState state)
    {
        return state.Execute(new StatsCommand());
    }
}

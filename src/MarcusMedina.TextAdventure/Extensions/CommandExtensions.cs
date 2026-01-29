// <copyright file="CommandExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Extensions;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;

public static class CommandExtensions
{
    /// <summary>
    /// Executes a command against the provided game state.
    /// </summary>
    public static CommandResult Execute(this GameState state, ICommand command)
    {
        var result = command.Execute(new CommandContext(state));
        if (!result.ShouldQuit)
        {
            state.TimeSystem.Tick(state);
            state.RandomEvents.Tick(state);
        }

        return result;
    }

    /// <summary>
    /// Executes the built-in LookCommand for the current state.
    /// </summary>
    public static CommandResult Look(this GameState state) => state.Execute(new LookCommand());

    /// <summary>
    /// Executes the built-in InventoryCommand for the current state.
    /// </summary>
    public static CommandResult InventoryView(this GameState state) => state.Execute(new InventoryCommand());

    /// <summary>
    /// Executes the built-in StatsCommand for the current state.
    /// </summary>
    public static CommandResult StatsView(this GameState state) => state.Execute(new StatsCommand());
}

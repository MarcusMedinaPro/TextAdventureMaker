// <copyright file="CommandExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

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

    /// <summary>
    /// Prints the current room: name, description, items, NPCs, and exits.
    /// </summary>
    public static void ShowRoom(this GameState state, ILanguageProvider? provider = null) =>
        state.CurrentLocation.ShowRoom(provider: provider);

    /// <summary>
    /// Prints the room name header followed by the command result message and reactions.
    /// Useful for displaying look results with a room title.
    /// </summary>
    public static void ShowLookResult(this GameState state, CommandResult result)
    {
        Console.WriteLine($"Room: {state.CurrentLocation.Id.ToProperCase()}");
        result.WriteToConsole();
    }

    /// <summary>
    /// Displays a command result using context-aware formatting:
    /// LookCommand shows room name + result, all others show result only.
    /// Automatically refreshes room display after go/move/load commands.
    /// </summary>
    public static void DisplayResult(this GameState state, ICommand command, CommandResult result)
    {
        if (command is LookCommand)
            state.ShowLookResult(result);
        else
            result.WriteToConsole();

        if (result.ShouldAutoLook(command))
            state.ShowRoom();
    }
}

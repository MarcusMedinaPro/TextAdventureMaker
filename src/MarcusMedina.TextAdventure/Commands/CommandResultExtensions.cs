// <copyright file="CommandResultExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public static class CommandResultExtensions
{
    /// <summary>Prefix a "did you mean" hint to the result message.</summary>
    public static CommandResult WithSuggestion(this CommandResult result, string suggestion)
    {
        string hint = Language.DidYouMean(suggestion);
        string message = string.IsNullOrWhiteSpace(result.Message)
            ? hint
            : $"{hint}\n{result.Message}";

        return new CommandResult(
            result.Success,
            message,
            result.Error,
            result.ShouldQuit,
            result.ReactionsList);
    }

    /// <summary>
    /// Writes the result message and any reactions to the console.
    /// Reactions are prefixed with "&gt; ".
    /// </summary>
    public static void WriteToConsole(this CommandResult result)
    {
        if (!string.IsNullOrWhiteSpace(result.Message))
            Console.WriteLine(result.Message);

        foreach (string reaction in result.ReactionsList)
        {
            if (!string.IsNullOrWhiteSpace(reaction))
                Console.WriteLine($"> {reaction}");
        }
    }

    /// <summary>
    /// Returns true when the command result indicates the room display
    /// should be refreshed (after go, move, or load).
    /// </summary>
    public static bool ShouldAutoLook(this CommandResult result, ICommand command) =>
        result.Success && !result.ShouldQuit && command is GoCommand or MoveCommand or LoadCommand;
}

// <copyright file="CommandResultExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public static class CommandResultExtensions
{
    /// <summary>Prefix a "did you mean" hint to the result message.</summary>
    public static CommandResult WithSuggestion(this CommandResult result, string suggestion)
    {
        var hint = Language.DidYouMean(suggestion);
        var message = string.IsNullOrWhiteSpace(result.Message)
            ? hint
            : $"{hint}\n{result.Message}";

        return new CommandResult(
            result.Success,
            message,
            result.Error,
            result.ShouldQuit,
            result.ReactionsList);
    }
}

// <copyright file="FuzzyCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

public sealed class FuzzyCommand(ICommand inner, string suggestion) : ICommand
{
    private readonly ICommand _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private readonly string _suggestion = suggestion ?? throw new ArgumentNullException(nameof(suggestion));

    /// <summary>Execute the inner command and prefix a suggestion message.</summary>
    public CommandResult Execute(CommandContext context)
    {
        var result = _inner.Execute(context);
        var hint = Language.DidYouMean(_suggestion);
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

// <copyright file="FuzzyCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public sealed class FuzzyCommand : ICommand
{
    private readonly ICommand _inner;
    private readonly string _suggestion;

    /// <summary>Create a wrapper that prefixes a "did you mean" hint.</summary>
    public FuzzyCommand(ICommand inner, string suggestion)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _suggestion = suggestion ?? throw new ArgumentNullException(nameof(suggestion));
    }

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

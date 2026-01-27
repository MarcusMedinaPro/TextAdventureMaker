// <copyright file="MoveCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

/// <summary>Move or push an item in the current location.</summary>
public class MoveCommand : ICommand
{
    public string Target { get; }

    public MoveCommand(string target)
    {
        Target = target;
    }

    public CommandResult Execute(CommandContext context)
    {
        if (string.IsNullOrWhiteSpace(Target))
        {
            return CommandResult.Fail(Language.NothingToMove, GameError.MissingArgument);
        }

        var location = context.State.CurrentLocation;
        var item = location.FindItem(Target);
        string? suggestion = null;

        if (item == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(Target))
        {
            var best = FuzzyMatcher.FindBestItem(location.Items, Target, context.State.FuzzyMaxDistance);
            if (best != null)
            {
                item = best;
                suggestion = best.Name;
            }
        }

        if (item == null)
        {
            return CommandResult.Fail(Language.NoSuchItemHere, GameError.ItemNotFound);
        }

        var moveFailed = item.GetReaction(ItemAction.MoveFailed);
        if (!string.IsNullOrWhiteSpace(moveFailed))
        {
            var failed = CommandResult.Fail(Language.CannotMoveItem, GameError.ItemNotUsable, moveFailed);
            return suggestion != null ? failed.WithSuggestion(suggestion) : failed;
        }

        if (item.Takeable && item.GetReaction(ItemAction.Move) == null)
        {
            var failed = CommandResult.Fail(Language.CanTakeInstead(item.Name), GameError.ItemNotUsable);
            return suggestion != null ? failed.WithSuggestion(suggestion) : failed;
        }

        item.Move();
        var onMove = item.GetReaction(ItemAction.Move);
        var ok = onMove != null
            ? CommandResult.Ok(Language.MoveItem(item.Name), onMove)
            : CommandResult.Ok(Language.MoveItem(item.Name));

        return suggestion != null ? ok.WithSuggestion(suggestion) : ok;
    }
}

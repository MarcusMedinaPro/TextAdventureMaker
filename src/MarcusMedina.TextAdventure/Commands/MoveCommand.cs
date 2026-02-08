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

        ILocation location = context.State.CurrentLocation;
        IItem? item = location.FindItem(Target);
        string? suggestion = null;

        if (item == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(Target))
        {
            IItem? best = FuzzyMatcher.FindBestItem(location.Items, Target, context.State.FuzzyMaxDistance);
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

        string? moveFailed = item.GetReaction(ItemAction.MoveFailed);
        if (!string.IsNullOrWhiteSpace(moveFailed))
        {
            CommandResult failed = CommandResult.Fail(Language.CannotMoveItem, GameError.ItemNotUsable, moveFailed);
            return suggestion != null ? failed.WithSuggestion(suggestion) : failed;
        }

        string displayName = Language.EntityName(item);
        if (item.Takeable && item.GetReaction(ItemAction.Move) == null)
        {
            CommandResult failed = CommandResult.Fail(Language.CanTakeInstead(displayName), GameError.ItemNotUsable);
            return suggestion != null ? failed.WithSuggestion(suggestion) : failed;
        }

        item.Move();
        context.State.Events.Publish(new GameEvent(GameEventType.MoveItem, context.State, location, item));
        string? onMove = item.GetReaction(ItemAction.Move);
        CommandResult ok = onMove != null
            ? CommandResult.Ok(Language.MoveItem(displayName), onMove)
            : CommandResult.Ok(Language.MoveItem(displayName));

        return suggestion != null ? ok.WithSuggestion(suggestion) : ok;
    }
}

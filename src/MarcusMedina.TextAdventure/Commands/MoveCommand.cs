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
        (item, suggestion) = FuzzyItemResolver.Resolve(context.State, location.Items, item, Target);

        if (item  is null)
        {
            return CommandResult.Fail(Language.NoSuchItemHere, GameError.ItemNotFound);
        }

        string? moveFailed = item.GetReaction(ItemAction.MoveFailed);
        if (!string.IsNullOrWhiteSpace(moveFailed))
            return CommandResult.Fail(Language.CannotMoveItem, GameError.ItemNotUsable, moveFailed).WithOptionalSuggestion(suggestion);

        string displayName = Language.EntityName(item);
        if (item.Takeable && item.GetReaction(ItemAction.Move)  is null)
            return CommandResult.Fail(Language.CanTakeInstead(displayName), GameError.ItemNotUsable).WithOptionalSuggestion(suggestion);

        item.Move();
        context.State.Events.Publish(new GameEvent(GameEventType.MoveItem, context.State, location, item));
        string? onMove = item.GetReaction(ItemAction.Move);
        return CommandResultExtensions.OkWithReaction(Language.MoveItem(displayName), onMove).WithOptionalSuggestion(suggestion);
    }
}

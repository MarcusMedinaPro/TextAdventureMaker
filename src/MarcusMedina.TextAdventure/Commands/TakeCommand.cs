// <copyright file="TakeCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

public class TakeCommand : ICommand
{
    public string ItemName { get; }

    public TakeCommand(string itemName) => ItemName = itemName;

    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        var item = location.FindItem(ItemName);
        string? suggestion = null;
        if (item == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(ItemName))
        {
            var best = FuzzyMatcher.FindBestItem(location.Items, ItemName, context.State.FuzzyMaxDistance);
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

        if (!item.Takeable)
        {
            var reaction = item.GetReaction(ItemAction.TakeFailed);
            return reaction != null
                ? CommandResult.Fail(Language.CannotTakeItem, GameError.ItemNotTakeable, reaction)
                : CommandResult.Fail(Language.CannotTakeItem, GameError.ItemNotTakeable);
        }

        var inventory = context.State.Inventory;
        if (!inventory.Add(item))
        {
            return inventory.LimitType == InventoryLimitType.ByCount
                ? CommandResult.Fail(Language.InventoryFull, GameError.InventoryFull)
                : CommandResult.Fail(Language.TooHeavy, GameError.ItemTooHeavy);
        }

        _ = location.RemoveItem(item);
        item.Take();
        context.State.Events.Publish(new GameEvent(GameEventType.PickupItem, context.State, location, item));

        var onTake = item.GetReaction(ItemAction.Take);
        var result = onTake != null
            ? CommandResult.Ok(Language.TakeItem(item.Name), onTake)
            : CommandResult.Ok(Language.TakeItem(item.Name));

        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}

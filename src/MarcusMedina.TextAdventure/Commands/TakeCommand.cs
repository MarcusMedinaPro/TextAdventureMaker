// <copyright file="TakeCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class TakeCommand : ICommand
{
    public string ItemName { get; }

    public TakeCommand(string itemName)
    {
        ItemName = itemName;
    }

    public CommandResult Execute(CommandContext context)
    {
        ILocation location = context.State.CurrentLocation;
        IItem? item = location.FindItem(ItemName);
        string? suggestion = null;
        if (item == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(ItemName))
        {
            IItem? best = FuzzyMatcher.FindBestItem(location.Items, ItemName, context.State.FuzzyMaxDistance);
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
            string? reaction = item.GetReaction(ItemAction.TakeFailed);
            return reaction != null
                ? CommandResult.Fail(Language.CannotTakeItem, GameError.ItemNotTakeable, reaction)
                : CommandResult.Fail(Language.CannotTakeItem, GameError.ItemNotTakeable);
        }

        IInventory inventory = context.State.Inventory;
        if (!inventory.Add(item))
        {
            return inventory.LimitType == InventoryLimitType.ByCount
                ? CommandResult.Fail(Language.InventoryFull, GameError.InventoryFull)
                : CommandResult.Fail(Language.TooHeavy, GameError.ItemTooHeavy);
        }

        _ = location.RemoveItem(item);
        item.Take();
        context.State.Events.Publish(new GameEvent(GameEventType.PickupItem, context.State, location, item));

        string displayName = Language.EntityName(item);
        string? onTake = item.GetReaction(ItemAction.Take);
        CommandResult result = onTake != null
            ? CommandResult.Ok(Language.TakeItem(displayName), onTake)
            : CommandResult.Ok(Language.TakeItem(displayName));

        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}

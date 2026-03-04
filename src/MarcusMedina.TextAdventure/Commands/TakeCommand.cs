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
    public int? Amount { get; }

    public TakeCommand(string itemName, int? amount = null)
    {
        ItemName = itemName;
        Amount = amount;
    }

    public CommandResult Execute(CommandContext context)
    {
        ILocation location = context.State.CurrentLocation;
        IItem? item = location.FindItem(ItemName);
        string? suggestion = null;
        (item, suggestion) = FuzzyItemResolver.Resolve(context.State, location.Items, item, ItemName);

        if (item  is null)
            return CommandResult.Fail(Language.NoSuchItemHere, GameError.ItemNotFound);

        if (!item.Takeable)
        {
            string? reaction = item.GetReaction(ItemAction.TakeFailed);
            return reaction  is not null
                ? CommandResult.Fail(Language.CannotTakeItem, GameError.ItemNotTakeable, reaction)
                : CommandResult.Fail(Language.CannotTakeItem, GameError.ItemNotTakeable);
        }

        IInventory inventory = context.State.Inventory;

        // Partial stack take
        if (Amount.HasValue && item.IsStackable && item.Amount.HasValue && Amount.Value < item.Amount.Value)
        {
            IItem? split = item.Clone();
            if (split  is null)
                return CommandResult.Fail(Language.NoSuchItemHere, GameError.ItemNotFound);

            split.SetAmount(Amount.Value);
            if (!inventory.Add(split))
                return inventory.LimitType == InventoryLimitType.ByCount
                    ? CommandResult.Fail(Language.InventoryFull, GameError.InventoryFull)
                    : CommandResult.Fail(Language.TooHeavy, GameError.ItemTooHeavy);

            item.SetAmount(item.Amount.Value - Amount.Value);
            split.Take();
            context.State.Events.Publish(new GameEvent(GameEventType.PickupItem, context.State, location, split));

            string splitDisplay = $"{Amount.Value} {item.Name}";
            string? onTake = item.GetReaction(ItemAction.Take);
            return CommandResultExtensions.OkWithReaction(Language.TakeItem(splitDisplay), onTake).WithOptionalSuggestion(suggestion);
        }

        // Full take (normal or full stack)
        if (!inventory.Add(item))
            return inventory.LimitType == InventoryLimitType.ByCount
                ? CommandResult.Fail(Language.InventoryFull, GameError.InventoryFull)
                : CommandResult.Fail(Language.TooHeavy, GameError.ItemTooHeavy);

        _ = location.RemoveItem(item);
        item.Take();
        context.State.Events.Publish(new GameEvent(GameEventType.PickupItem, context.State, location, item));

        string displayName = Language.EntityName(item);
        string? fullOnTake = item.GetReaction(ItemAction.Take);
        return CommandResultExtensions.OkWithReaction(Language.TakeItem(displayName), fullOnTake).WithOptionalSuggestion(suggestion);
    }
}

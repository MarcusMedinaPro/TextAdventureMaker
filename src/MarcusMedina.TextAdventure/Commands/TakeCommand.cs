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
            return CommandResult.Fail(Language.NoSuchItemHere, GameError.ItemNotFound);

        if (!item.Takeable)
        {
            string? reaction = item.GetReaction(ItemAction.TakeFailed);
            return reaction != null
                ? CommandResult.Fail(Language.CannotTakeItem, GameError.ItemNotTakeable, reaction)
                : CommandResult.Fail(Language.CannotTakeItem, GameError.ItemNotTakeable);
        }

        IInventory inventory = context.State.Inventory;

        // Partial stack take
        if (Amount.HasValue && item.IsStackable && item.Amount.HasValue && Amount.Value < item.Amount.Value)
        {
            IItem? split = item.Clone();
            if (split == null)
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
            CommandResult splitResult = onTake != null
                ? CommandResult.Ok(Language.TakeItem(splitDisplay), onTake)
                : CommandResult.Ok(Language.TakeItem(splitDisplay));

            return suggestion != null ? splitResult.WithSuggestion(suggestion) : splitResult;
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
        CommandResult result = fullOnTake != null
            ? CommandResult.Ok(Language.TakeItem(displayName), fullOnTake)
            : CommandResult.Ok(Language.TakeItem(displayName));

        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}

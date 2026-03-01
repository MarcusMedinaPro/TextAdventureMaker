// <copyright file="DropCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class DropCommand : ICommand
{
    public string ItemName { get; }
    public int? Amount { get; }

    public DropCommand(string itemName, int? amount = null)
    {
        ItemName = itemName;
        Amount = amount;
    }

    public CommandResult Execute(CommandContext context)
    {
        IItem? item = context.State.Inventory.FindItem(ItemName);
        string? suggestion = null;
        if (item == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(ItemName))
        {
            IItem? best = FuzzyMatcher.FindBestItem(context.State.Inventory.Items, ItemName, context.State.FuzzyMaxDistance);
            if (best != null)
            {
                item = best;
                suggestion = best.Name;
            }
        }

        if (item == null)
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotInInventory);

        ILocation location = context.State.CurrentLocation;

        // Partial stack drop
        if (Amount.HasValue && item.IsStackable && item.Amount.HasValue && Amount.Value < item.Amount.Value)
        {
            item.SetAmount(item.Amount.Value - Amount.Value);

            IItem split = item.Clone();
            split.SetAmount(Amount.Value);
            location.AddItem(split);
            split.Drop();
            context.State.Events.Publish(new GameEvent(GameEventType.DropItem, context.State, location, split));

            string splitDisplay = $"{Amount.Value} {item.Name}";
            string? onDrop = item.GetReaction(ItemAction.Drop);
            CommandResult splitResult = onDrop != null
                ? CommandResult.Ok(Language.DropItem(splitDisplay), onDrop)
                : CommandResult.Ok(Language.DropItem(splitDisplay));

            return suggestion != null ? splitResult.WithSuggestion(suggestion) : splitResult;
        }

        // Full drop
        _ = context.State.Inventory.Remove(item);
        location.AddItem(item);
        item.Drop();
        context.State.Events.Publish(new GameEvent(GameEventType.DropItem, context.State, location, item));

        string displayName = Language.EntityName(item);
        string? fullOnDrop = item.GetReaction(ItemAction.Drop);
        CommandResult result = fullOnDrop != null
            ? CommandResult.Ok(Language.DropItem(displayName), fullOnDrop)
            : CommandResult.Ok(Language.DropItem(displayName));

        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}

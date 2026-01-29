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

    public DropCommand(string itemName)
    {
        ItemName = itemName;
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
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotInInventory);
        }

        _ = context.State.Inventory.Remove(item);
        context.State.CurrentLocation.AddItem(item);
        item.Drop();
        context.State.Events.Publish(new GameEvent(GameEventType.DropItem, context.State, context.State.CurrentLocation, item));

        string? onDrop = item.GetReaction(ItemAction.Drop);
        CommandResult result = onDrop != null
            ? CommandResult.Ok(Language.DropItem(item.Name), onDrop)
            : CommandResult.Ok(Language.DropItem(item.Name));

        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}

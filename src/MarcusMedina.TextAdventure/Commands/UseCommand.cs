// <copyright file="UseCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using System.Linq;

namespace MarcusMedina.TextAdventure.Commands;

public class UseCommand : ICommand
{
    public string ItemName { get; }

    public UseCommand(string itemName)
    {
        ItemName = itemName;
    }

    public CommandResult Execute(CommandContext context)
    {
        ILocation location = context.State.CurrentLocation;
        IItem? item = context.State.Inventory.FindItem(ItemName);
        bool useFixedRoomItem = false;
        string? suggestion = null;

        if (item  is null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(ItemName))
        {
            IItem? best = FuzzyMatcher.FindBestItem(context.State.Inventory.Items, ItemName, context.State.FuzzyMaxDistance);
            if (best  is not null)
            {
                item = best;
                suggestion = best.Name;
            }
        }

        if (item is null)
        {
            IItem? roomItem = location.FindItem(ItemName);
            if (roomItem is not null && !roomItem.Takeable)
            {
                item = roomItem;
                useFixedRoomItem = true;
            }
        }

        if (item is null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(ItemName))
        {
            IItem? bestRoomFixture = FuzzyMatcher.FindBestItem(
                location.Items.Where(current => !current.Takeable),
                ItemName,
                context.State.FuzzyMaxDistance);

            if (bestRoomFixture is not null)
            {
                item = bestRoomFixture;
                useFixedRoomItem = true;
                suggestion = bestRoomFixture.Name;
            }
        }

        if (item  is null)
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotFound);
        }

        item.Use();
        if (!useFixedRoomItem && item.Amount.HasValue && item.Amount.Value == 0)
        {
            _ = context.State.Inventory.Remove(item);
        }
        string displayName = Language.EntityName(item);
        string? onUse = item.GetReaction(ItemAction.Use);
        CommandResult result = onUse  is not null
            ? CommandResult.Ok(Language.UseItem(displayName), onUse)
            : CommandResult.Ok(Language.UseItem(displayName));

        return suggestion  is not null ? result.WithSuggestion(suggestion) : result;
    }
}

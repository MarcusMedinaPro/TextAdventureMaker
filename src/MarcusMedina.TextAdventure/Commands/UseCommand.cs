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

        (item, suggestion) = FuzzyItemResolver.Resolve(context.State, context.State.Inventory.Items, item, ItemName);

        if (item is null)
        {
            IItem? roomItem = location.FindItem(ItemName);
            if (roomItem is not null && !roomItem.Takeable)
            {
                item = roomItem;
                useFixedRoomItem = true;
            }
        }

        if (item is null)
        {
            IEnumerable<IItem> fixtures = location.Items.Where(current => !current.Takeable);
            (IItem? bestFixture, string? fixtureSuggestion) = FuzzyItemResolver.Resolve(context.State, fixtures, null, ItemName);
            if (bestFixture is not null)
            {
                item = bestFixture;
                useFixedRoomItem = true;
                suggestion = fixtureSuggestion;
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
        return CommandResultExtensions.OkWithReaction(Language.UseItem(displayName), onUse).WithOptionalSuggestion(suggestion);
    }
}

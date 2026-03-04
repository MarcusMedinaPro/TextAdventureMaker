// <copyright file="EatCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class EatCommand(string itemName) : ICommand
{
    public string ItemName { get; } = itemName;

    public CommandResult Execute(CommandContext context)
    {
        IItem? item = context.State.Inventory.FindItem(ItemName);
        string? suggestion = null;
        (item, suggestion) = FuzzyItemResolver.Resolve(context.State, context.State.Inventory.Items, item, ItemName);

        if (item is null)
        {
            IItem? roomItem = context.State.CurrentLocation.FindItem(ItemName);
            if (roomItem is null)
                (roomItem, _) = FuzzyItemResolver.Resolve(context.State, context.State.CurrentLocation.Items, null, ItemName);
            if (roomItem is not null)
                return CommandResult.Fail(Language.MustPickUpToEat, GameError.ItemNotFound);
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotFound);
        }

        if (!item.IsFood)
            return CommandResult.Fail(Language.CannotEatThat, GameError.ItemNotUsable);

        return ConsumableItemHandler.Apply(item, context, Language.EatItem(Language.EntityName(item)), suggestion);
    }
}

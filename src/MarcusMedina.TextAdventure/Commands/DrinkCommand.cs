// <copyright file="DrinkCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class DrinkCommand(string itemName) : ICommand
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
                return CommandResult.Fail(Language.MustPickUpToDrink, GameError.ItemNotFound);
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotFound);
        }

        if (!item.IsDrinkable)
            return CommandResult.Fail(Language.CannotDrinkThat, GameError.ItemNotUsable);

        return ConsumableItemHandler.Apply(item, context, Language.DrinkItem(Language.EntityName(item)), suggestion);
    }
}

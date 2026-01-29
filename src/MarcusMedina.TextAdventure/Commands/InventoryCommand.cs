// <copyright file="InventoryCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class InventoryCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        IInventory inventory = context.State.Inventory;
        if (inventory.Count == 0)
        {
            string emptyMessage = $"{Language.InventoryLabel}{Language.None}";
            if (inventory.TotalWeight > 0)
            {
                emptyMessage += $"\n{Language.TotalWeight(inventory.TotalWeight)}";
            }

            return CommandResult.Ok(emptyMessage);
        }

        IEnumerable<string> items = inventory.Items
            .Select(i => Language.ItemWithWeight(i.Name, i.Weight));

        string message = $"{Language.InventoryLabel}{items.CommaJoin()}";
        if (inventory.TotalWeight > 0)
        {
            message += $"\n{Language.TotalWeight(inventory.TotalWeight)}";
        }

        return CommandResult.Ok(message);
    }
}

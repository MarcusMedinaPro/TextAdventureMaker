// <copyright file="InventoryCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

public class InventoryCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var inventory = context.State.Inventory;
        if (inventory.Count == 0)
        {
            var emptyMessage = $"{Language.InventoryLabel}{Language.None}";
            if (inventory.TotalWeight > 0)
            {
                emptyMessage += $"\n{Language.TotalWeight(inventory.TotalWeight)}";
            }

            return CommandResult.Ok(emptyMessage);
        }

        var items = inventory.Items
            .Select(i => Language.ItemWithWeight(i.Name, i.Weight));

        var message = $"{Language.InventoryLabel}{items.CommaJoin()}";
        if (inventory.TotalWeight > 0)
        {
            message += $"\n{Language.TotalWeight(inventory.TotalWeight)}";
        }

        return CommandResult.Ok(message);
    }
}

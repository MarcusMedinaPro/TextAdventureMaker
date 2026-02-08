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

        IEnumerable<string> items = FormatItems(inventory.Items);

        string message = $"{Language.InventoryLabel}{items.CommaJoin()}";
        if (inventory.TotalWeight > 0)
        {
            message += $"\n{Language.TotalWeight(inventory.TotalWeight)}";
        }

        return CommandResult.Ok(message);
    }

    private static IEnumerable<string> FormatItems(IEnumerable<IItem> items)
    {
        List<IItem> materialised = items.ToList();
        IEnumerable<IGrouping<string, IItem>> stacked = materialised
            .Where(item => item.IsStackable)
            .GroupBy(item => item.Id, StringComparer.OrdinalIgnoreCase);

        foreach (IGrouping<string, IItem> group in stacked)
        {
            IItem sample = group.First();
            int amount = group.Sum(item => item.Amount ?? 1);
            string name = Language.EntityName(sample);
            if (amount > 1 || sample.Amount.HasValue)
            {
                name = $"{name} ({amount})";
            }

            yield return Language.ItemWithWeight(name, sample.Weight);
        }

        foreach (IItem item in materialised.Where(item => !item.IsStackable))
        {
            string name = Language.EntityName(item);
            if (item.Amount.HasValue)
            {
                name = $"{name} ({item.Amount.Value})";
            }

            yield return Language.ItemWithWeight(name, item.Weight);
        }
    }
}

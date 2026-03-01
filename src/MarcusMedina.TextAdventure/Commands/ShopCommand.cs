// <copyright file="ShopCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using System.Text;

namespace MarcusMedina.TextAdventure.Commands;

/// <summary>
/// Command to display a store's inventory and prices.
/// </summary>
public sealed class ShopCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var store = context.State.CurrentLocation.GetStore();
        if (store is null)
            return CommandResult.Fail("There's no store here.", GameError.InvalidState);

        if (!store.IsOpen)
            return CommandResult.Ok($"{store.Name} is closed.");

        var sb = new StringBuilder();
        sb.AppendLine($"=== {store.Name} ===");
        sb.AppendLine(store.Description);
        sb.AppendLine();
        sb.AppendLine("For sale:");

        var availableItems = store.Inventory.Where(i => i.Stock != 0).ToList();
        if (availableItems.Count == 0)
        {
            sb.AppendLine("  (no items in stock)");
        }
        else
        {
            foreach (var item in availableItems)
            {
                var price = (int)(item.BasePrice * store.PriceModifier);
                var stock = item.Stock == -1 ? "∞" : item.Stock.ToString();
                sb.AppendLine($"  {item.Name} - {price} gold (stock: {stock})");
            }
        }

        sb.AppendLine();
        sb.AppendLine($"Your gold: {context.State.Wallet.GetBalance(store.CurrencyId)}");

        return CommandResult.Ok(sb.ToString());
    }
}

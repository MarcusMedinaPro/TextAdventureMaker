// <copyright file="SellCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Commands;

/// <summary>
/// Command to sell an item to a store.
/// </summary>
public sealed class SellCommand(string itemName) : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var store = context.State.CurrentLocation.GetStore();
        if (store is null)
            return CommandResult.Fail("There's no store here.", GameError.InvalidState);

        var item = context.State.Inventory.FindItem(itemName);
        if (item is null)
            return CommandResult.Fail($"You don't have a {itemName}.", GameError.ItemNotInInventory);

        var result = store.TrySell(context.State, item);
        return result.Success
            ? CommandResult.Ok(result.Message)
            : CommandResult.Fail(result.Message, GameError.InvalidState);
    }
}

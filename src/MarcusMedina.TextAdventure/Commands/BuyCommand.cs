// <copyright file="BuyCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Commands;

/// <summary>
/// Command to purchase an item from a store.
/// </summary>
public sealed class BuyCommand(string itemName) : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var store = context.State.CurrentLocation.GetStore();
        if (store is null)
            return CommandResult.Fail("There's no store here.", GameError.InvalidState);

        var result = store.TryBuy(context.State, itemName);
        return result.Success
            ? CommandResult.Ok(result.Message)
            : CommandResult.Fail(result.Message, GameError.InvalidState);
    }
}

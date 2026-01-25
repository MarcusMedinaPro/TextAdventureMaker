// <copyright file="DropCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class DropCommand : ICommand
{
    public string ItemName { get; }

    public DropCommand(string itemName)
    {
        ItemName = itemName;
    }

    public CommandResult Execute(CommandContext context)
    {
        var item = context.State.Inventory.FindItem(ItemName);
        if (item == null)
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotInInventory);
        }

        context.State.Inventory.Remove(item);
        context.State.CurrentLocation.AddItem(item);
        item.Drop();
        context.State.Events.Publish(new GameEvent(GameEventType.DropItem, context.State, context.State.CurrentLocation, item));

        var onDrop = item.GetReaction(ItemAction.Drop);
        return onDrop != null
            ? CommandResult.Ok(Language.DropItem(item.Name), onDrop)
            : CommandResult.Ok(Language.DropItem(item.Name));
    }
}

// <copyright file="DropAllCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class DropAllCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        IInventory inventory = context.State.Inventory;
        if (inventory.Count == 0)
        {
            return CommandResult.Fail(Language.NothingToDrop, GameError.ItemNotInInventory);
        }

        ILocation location = context.State.CurrentLocation;
        List<IItem> items = inventory.Items.ToList();
        List<string> reactions = [];

        foreach (IItem? item in items)
        {
            _ = inventory.Remove(item);
            location.AddItem(item);
            item.Drop();
            context.State.Events.Publish(new GameEvent(GameEventType.DropItem, context.State, location, item));
            string? reaction = item.GetReaction(ItemAction.Drop);
            if (!string.IsNullOrWhiteSpace(reaction))
            {
                reactions.Add(reaction);
            }
        }

        string list = items.Select(i => i.Name).CommaJoin();
        return reactions.Count > 0
            ? CommandResult.Ok(Language.DropAll(list), reactions.ToArray())
            : CommandResult.Ok(Language.DropAll(list));
    }
}

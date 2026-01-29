// <copyright file="DropAllCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

public class DropAllCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var inventory = context.State.Inventory;
        if (inventory.Count == 0)
        {
            return CommandResult.Fail(Language.NothingToDrop, GameError.ItemNotInInventory);
        }

        var location = context.State.CurrentLocation;
        var items = inventory.Items.ToList();
        var reactions = new List<string>();

        foreach (var item in items)
        {
            _ = inventory.Remove(item);
            location.AddItem(item);
            item.Drop();
            context.State.Events.Publish(new GameEvent(GameEventType.DropItem, context.State, location, item));
            var reaction = item.GetReaction(ItemAction.Drop);
            if (!string.IsNullOrWhiteSpace(reaction))
            {
                reactions.Add(reaction);
            }
        }

        var list = items.Select(i => i.Name).CommaJoin();
        return reactions.Count > 0
            ? CommandResult.Ok(Language.DropAll(list), reactions.ToArray())
            : CommandResult.Ok(Language.DropAll(list));
    }
}

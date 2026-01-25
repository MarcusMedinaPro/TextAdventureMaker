// <copyright file="TakeAllCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class TakeAllCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        var inventory = context.State.Inventory;

        var candidates = location.Items.Where(i => i.Takeable).ToList();
        if (candidates.Count == 0)
        {
            return CommandResult.Fail(Language.NothingToTake, GameError.ItemNotFound);
        }

        var taken = new List<IItem>();
        var skipped = new List<IItem>();
        var reactions = new List<string>();

        foreach (var item in candidates)
        {
            if (inventory.Add(item))
            {
                location.RemoveItem(item);
                item.Take();
                taken.Add(item);
                context.State.Events.Publish(new GameEvent(GameEventType.PickupItem, context.State, location, item));
                var reaction = item.GetReaction(ItemAction.Take);
                if (!string.IsNullOrWhiteSpace(reaction))
                {
                    reactions.Add(reaction);
                }
            }
            else
            {
                skipped.Add(item);
            }
        }

        if (taken.Count == 0)
        {
            if (inventory.LimitType == InventoryLimitType.ByCount)
            {
                return CommandResult.Fail(Language.InventoryFull, GameError.InventoryFull);
            }

            return CommandResult.Fail(Language.TooHeavy, GameError.ItemTooHeavy);
        }

        var takenList = taken.Select(i => i.Name).CommaJoin();
        var message = Language.TakeAll(takenList);

        if (skipped.Count > 0)
        {
            var skippedList = skipped.Select(i => i.Name).CommaJoin();
            message = $"{message}\n{Language.TakeAllSkipped(skippedList)}";
        }

        return reactions.Count > 0
            ? CommandResult.Ok(message, reactions.ToArray())
            : CommandResult.Ok(message);
    }
}

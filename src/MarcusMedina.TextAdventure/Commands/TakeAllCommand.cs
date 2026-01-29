// <copyright file="TakeAllCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class TakeAllCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        ILocation location = context.State.CurrentLocation;
        IInventory inventory = context.State.Inventory;

        List<IItem> candidates = location.Items.Where(i => i.Takeable).ToList();
        if (candidates.Count == 0)
        {
            return CommandResult.Fail(Language.NothingToTake, GameError.ItemNotFound);
        }

        List<IItem> taken = [];
        List<IItem> skipped = [];
        List<string> reactions = [];

        foreach (IItem? item in candidates)
        {
            if (inventory.Add(item))
            {
                _ = location.RemoveItem(item);
                item.Take();
                taken.Add(item);
                context.State.Events.Publish(new GameEvent(GameEventType.PickupItem, context.State, location, item));
                string? reaction = item.GetReaction(ItemAction.Take);
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
            return inventory.LimitType == InventoryLimitType.ByCount
                ? CommandResult.Fail(Language.InventoryFull, GameError.InventoryFull)
                : CommandResult.Fail(Language.TooHeavy, GameError.ItemTooHeavy);
        }

        string takenList = taken.Select(i => i.Name).CommaJoin();
        string message = Language.TakeAll(takenList);

        if (skipped.Count > 0)
        {
            string skippedList = skipped.Select(i => i.Name).CommaJoin();
            message = $"{message}\n{Language.TakeAllSkipped(skippedList)}";
        }

        return reactions.Count > 0
            ? CommandResult.Ok(message, reactions.ToArray())
            : CommandResult.Ok(message);
    }
}

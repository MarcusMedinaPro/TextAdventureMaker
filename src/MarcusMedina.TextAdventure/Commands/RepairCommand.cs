// <copyright file="RepairCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Commands;

public class RepairCommand(string itemName, string? toolName = null) : ICommand
{
    public string Name => "repair";
    public string[]? Aliases => null;
    public string Description => "Repair a damaged item (requires repair kit)";

    public CommandResult Execute(CommandContext context)
    {
        IItem? item = context.State.CurrentLocation.FindItem(itemName);
        if (item  is null)
        {
            return CommandResult.Fail($"You don't see '{itemName}' here.", Enums.GameError.ItemNotFound);
        }

        if (!item.Durability.HasValue || !item.MaxDurability.HasValue)
        {
            return CommandResult.Fail($"The {item.Name} doesn't need repair.", Enums.GameError.None);
        }

        if (item.Durability.Value == item.MaxDurability.Value)
        {
            return CommandResult.Fail($"The {item.Name} is already in pristine condition.", Enums.GameError.None);
        }

        IItem? repairKit = context.State.Inventory.FindItem(toolName ?? "repair kit");
        if (repairKit  is null)
        {
            return CommandResult.Fail("You need a repair kit to repair items.", Enums.GameError.None);
        }

        // Restore durability
        _ = item.SetDurability(item.MaxDurability.Value, item.MaxDurability.Value);

        return CommandResult.Ok(
            $"You use the {repairKit.Name} to repair the {item.Name}. It's as good as new!",
            $"The {item.Name} is now {item.GetCondition()}.");
    }
}

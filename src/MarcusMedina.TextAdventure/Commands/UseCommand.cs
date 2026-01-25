// <copyright file="UseCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class UseCommand : ICommand
{
    public string ItemName { get; }

    public UseCommand(string itemName)
    {
        ItemName = itemName;
    }

    public CommandResult Execute(CommandContext context)
    {
        var item = context.State.Inventory.FindItem(ItemName);
        if (item == null)
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotFound);
        }

        item.Use();
        var onUse = item.GetReaction(ItemAction.Use);
        return onUse != null
            ? CommandResult.Ok(Language.UseItem(item.Name), onUse)
            : CommandResult.Ok(Language.UseItem(item.Name));
    }
}

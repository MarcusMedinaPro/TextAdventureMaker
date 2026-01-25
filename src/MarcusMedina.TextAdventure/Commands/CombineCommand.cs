// <copyright file="CombineCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class CombineCommand : ICommand
{
    public string Left { get; }
    public string Right { get; }

    public CombineCommand(string left, string right)
    {
        Left = left;
        Right = right;
    }

    public CommandResult Execute(CommandContext context)
    {
        var inventory = context.State.Inventory;
        var leftItem = inventory.FindItem(Left);
        var rightItem = inventory.FindItem(Right);

        if (leftItem == null || rightItem == null)
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotInInventory);
        }

        var result = context.State.RecipeBook.Combine(leftItem, rightItem);
        if (!result.Success)
        {
            return CommandResult.Fail(Language.CannotCombineItems, GameError.ItemNotUsable);
        }

        inventory.Remove(leftItem);
        inventory.Remove(rightItem);

        foreach (var created in result.Created)
        {
            inventory.Add(created);
        }

        return CommandResult.Ok(Language.CombineResult(leftItem.Name, rightItem.Name));
    }
}

// <copyright file="CombineCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

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
        IInventory inventory = context.State.Inventory;
        IItem? leftItem = inventory.FindItem(Left);
        IItem? rightItem = inventory.FindItem(Right);
        string? suggestion = null;

        if (context.State.EnableFuzzyMatching)
        {
            if (leftItem == null && !FuzzyMatcher.IsLikelyCommandToken(Left))
            {
                IItem? bestLeft = FuzzyMatcher.FindBestItem(inventory.Items, Left, context.State.FuzzyMaxDistance);
                if (bestLeft != null)
                {
                    leftItem = bestLeft;
                    suggestion ??= bestLeft.Name;
                }
            }

            if (rightItem == null && !FuzzyMatcher.IsLikelyCommandToken(Right))
            {
                IItem? bestRight = FuzzyMatcher.FindBestItem(inventory.Items, Right, context.State.FuzzyMaxDistance);
                if (bestRight != null)
                {
                    rightItem = bestRight;
                    suggestion ??= bestRight.Name;
                }
            }
        }

        if (leftItem == null || rightItem == null)
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotInInventory);
        }

        CombinationResult result = context.State.RecipeBook.Combine(leftItem, rightItem);
        if (!result.Success)
        {
            return CommandResult.Fail(Language.CannotCombineItems, GameError.ItemNotUsable);
        }

        _ = inventory.Remove(leftItem);
        _ = inventory.Remove(rightItem);

        foreach (IItem created in result.Created)
        {
            _ = inventory.Add(created);
        }

        CommandResult ok = CommandResult.Ok(Language.CombineResult(Language.EntityName(leftItem), Language.EntityName(rightItem)));
        return suggestion != null ? ok.WithSuggestion(suggestion) : ok;
    }
}

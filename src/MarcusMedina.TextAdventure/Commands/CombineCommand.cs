// <copyright file="CombineCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

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
        string? suggestion = null;

        if (context.State.EnableFuzzyMatching)
        {
            if (leftItem == null && !FuzzyMatcher.IsLikelyCommandToken(Left))
            {
                var bestLeft = FuzzyMatcher.FindBestItem(inventory.Items, Left, context.State.FuzzyMaxDistance);
                if (bestLeft != null)
                {
                    leftItem = bestLeft;
                    suggestion ??= bestLeft.Name;
                }
            }

            if (rightItem == null && !FuzzyMatcher.IsLikelyCommandToken(Right))
            {
                var bestRight = FuzzyMatcher.FindBestItem(inventory.Items, Right, context.State.FuzzyMaxDistance);
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

        var result = context.State.RecipeBook.Combine(leftItem, rightItem);
        if (!result.Success)
        {
            return CommandResult.Fail(Language.CannotCombineItems, GameError.ItemNotUsable);
        }

        _ = inventory.Remove(leftItem);
        _ = inventory.Remove(rightItem);

        foreach (var created in result.Created)
        {
            _ = inventory.Add(created);
        }

        var ok = CommandResult.Ok(Language.CombineResult(leftItem.Name, rightItem.Name));
        return suggestion != null ? ok.WithSuggestion(suggestion) : ok;
    }
}

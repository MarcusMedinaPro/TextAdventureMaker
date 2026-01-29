// <copyright file="UseCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

public class UseCommand : ICommand
{
    public string ItemName { get; }

    public UseCommand(string itemName) => ItemName = itemName;

    public CommandResult Execute(CommandContext context)
    {
        var item = context.State.Inventory.FindItem(ItemName);
        string? suggestion = null;
        if (item == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(ItemName))
        {
            var best = FuzzyMatcher.FindBestItem(context.State.Inventory.Items, ItemName, context.State.FuzzyMaxDistance);
            if (best != null)
            {
                item = best;
                suggestion = best.Name;
            }
        }

        if (item == null)
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotFound);
        }

        item.Use();
        var onUse = item.GetReaction(ItemAction.Use);
        var result = onUse != null
            ? CommandResult.Ok(Language.UseItem(item.Name), onUse)
            : CommandResult.Ok(Language.UseItem(item.Name));

        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}

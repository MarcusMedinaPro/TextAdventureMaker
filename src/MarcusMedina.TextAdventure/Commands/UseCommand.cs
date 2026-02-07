// <copyright file="UseCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
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
        IItem? item = context.State.Inventory.FindItem(ItemName);
        string? suggestion = null;
        if (item == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(ItemName))
        {
            IItem? best = FuzzyMatcher.FindBestItem(context.State.Inventory.Items, ItemName, context.State.FuzzyMaxDistance);
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
        string displayName = Language.EntityName(item);
        string? onUse = item.GetReaction(ItemAction.Use);
        CommandResult result = onUse != null
            ? CommandResult.Ok(Language.UseItem(displayName), onUse)
            : CommandResult.Ok(Language.UseItem(displayName));

        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}

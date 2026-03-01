// <copyright file="DrinkCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class DrinkCommand(string itemName) : ICommand
{
    public string ItemName { get; } = itemName;

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
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotFound);

        if (!item.IsDrinkable)
            return CommandResult.Fail(Language.CannotDrinkThat, GameError.ItemNotUsable);

        string displayName = Language.EntityName(item);
        item.Use();

        List<string> reactions = [];

        if (item.HealAmount > 0)
        {
            context.State.Stats.Heal(item.HealAmount);
            reactions.Add(Language.HealedAmount(item.HealAmount));
        }

        if (item.IsPoisoned && context.State is GameState gameState)
        {
            gameState.AddPoison(new PoisonEffect(displayName, item.PoisonDamagePerTurn, item.PoisonDurationTurns));
            reactions.Add(Language.PoisonedMessage);
        }

        if (item.Amount.HasValue && item.Amount.Value == 0)
            _ = context.State.Inventory.Remove(item);

        string? onUse = item.GetReaction(ItemAction.Use);
        if (onUse != null)
            reactions.Add(onUse);

        CommandResult result = reactions.Count > 0
            ? CommandResult.Ok(Language.DrinkItem(displayName), [.. reactions])
            : CommandResult.Ok(Language.DrinkItem(displayName));

        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}

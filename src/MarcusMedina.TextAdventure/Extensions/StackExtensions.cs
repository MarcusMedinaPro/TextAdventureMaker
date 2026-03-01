// <copyright file="StackExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Extensions;

public static class StackExtensions
{
    /// <summary>Create a stackable item with amount and weight-per-unit.</summary>
    public static Item AsStack(this Item item, int amount, float weightPerUnit = 0f) =>
        (Item)item.SetStackable()
                  .SetAmount(amount)
                  .SetWeight(weightPerUnit);

    /// <summary>Try to merge source into target stack. Returns true if merged.</summary>
    public static bool TryMerge(this IItem target, IItem source)
    {
        if (!target.IsStackable || !source.IsStackable)
            return false;
        if (!string.Equals(target.Id, source.Id, StringComparison.OrdinalIgnoreCase))
            return false;

        int total = (target.Amount ?? 1) + (source.Amount ?? 1);
        target.SetAmount(total);
        return true;
    }

    /// <summary>Split amount from a stack. Returns a new item with the split amount, or null if invalid.</summary>
    public static IItem? SplitStack(this IItem item, int amount)
    {
        if (!item.IsStackable || !item.Amount.HasValue)
            return null;
        if (amount <= 0 || amount >= item.Amount.Value)
            return null;

        IItem clone = item.Clone();
        clone.SetAmount(amount);
        item.SetAmount(item.Amount.Value - amount);
        return clone;
    }

    /// <summary>Format a stack item for display, e.g. "Arrows (x8)".</summary>
    public static string FormatStack(this IItem item) =>
        item is { IsStackable: true, Amount: > 1 }
            ? $"{item.Name} (x{item.Amount})"
            : item.Name;
}

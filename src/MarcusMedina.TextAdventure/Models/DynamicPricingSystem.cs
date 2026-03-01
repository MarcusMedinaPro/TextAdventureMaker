// <copyright file="DynamicPricingSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Calculates dynamic price modifiers based on game state factors.
/// </summary>
public sealed class DynamicPricingSystem
{
    /// <summary>
    /// Calculates the price modifier for a store based on reputation, supply, and time of day.
    /// </summary>
    public float CalculateModifier(IStore store, IGameState state)
    {
        var modifier = 1.0f;

        // Reputation affects price
        if (state.Factions is not null)
        {
            var reputation = state.Factions.GetReputation(store.Id);
            modifier *= reputation switch
            {
                >= 50 => 0.9f,    // 10% discount for high reputation
                >= 0 => 1.0f,     // Normal price
                _ => 1.2f         // 20% more expensive for low reputation
            };
        }

        // Supply/demand: low stock = more expensive
        var totalStock = store.Inventory.Sum(i => i.Stock == -1 ? 100 : i.Stock);
        if (totalStock < 10)
            modifier *= 1.3f;

        // Time of day affects pricing (night is more expensive)
        if (state.TimeSystem?.CurrentTimeOfDay == Enums.TimeOfDay.Night)
            modifier *= 1.1f;

        return modifier;
    }
}

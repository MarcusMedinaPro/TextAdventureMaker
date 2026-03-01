// <copyright file="ConsumableExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Extensions;

public static class ConsumableExtensions
{
    public static Item AsFood(this Item item, int healAmount) =>
        (Item)item.SetFood().SetHealAmount(healAmount);

    public static Item AsDrink(this Item item, int healAmount) =>
        (Item)item.SetDrinkable().SetHealAmount(healAmount);

    public static Item WithPoison(this Item item, int damagePerTurn, int turns) =>
        (Item)item.SetPoisoned().SetPoisonDamage(damagePerTurn, turns);
}

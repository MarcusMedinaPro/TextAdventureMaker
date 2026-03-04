// <copyright file="ConsumableExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Extensions;

public static class ConsumableExtensions
{
    public static IItem AsFood(this IItem item, int healAmount) =>
        item.SetFood().SetHealAmount(healAmount);

    public static IItem AsDrink(this IItem item, int healAmount) =>
        item.SetDrinkable().SetHealAmount(healAmount);

    public static IItem WithPoison(this IItem item, int damagePerTurn, int turns) =>
        item.SetPoisoned().SetPoisonDamage(damagePerTurn, turns);
}

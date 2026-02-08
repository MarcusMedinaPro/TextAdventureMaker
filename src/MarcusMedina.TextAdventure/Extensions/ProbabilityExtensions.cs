// <copyright file="ProbabilityExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Extensions;

public static class ProbabilityExtensions
{
    private static readonly Random Rng = new();

    public static bool PercentChance(this int percent)
    {
        return Rng.Next(100) < percent;
    }

    public static bool Chance(this double probability)
    {
        return Rng.NextDouble() < probability;
    }
}

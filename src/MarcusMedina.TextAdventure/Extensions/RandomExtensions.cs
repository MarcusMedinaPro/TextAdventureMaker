// <copyright file="RandomExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Extensions;

public static class RandomExtensions
{
    public static int Random(this int max) =>
        System.Random.Shared.Next(max + 1);

    public static int Random(this int max, int min) =>
        System.Random.Shared.Next(min, max + 1);

    public static int Dice(this int sides) =>
        System.Random.Shared.Next(1, sides + 1);

    public static int Dice(this int sides, int count)
    {
        int total = 0;
        for (int i = 0; i < count; i++)
        {
            total += sides.Dice();
        }

        return total;
    }
}

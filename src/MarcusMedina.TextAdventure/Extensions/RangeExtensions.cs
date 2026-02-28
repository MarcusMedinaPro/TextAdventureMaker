// <copyright file="RangeExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Extensions;

public static class RangeExtensions
{
    public static int Clamp(this int value, int min, int max) =>
        Math.Clamp(value, min, max);

    public static bool IsBetween(this int value, int min, int max) =>
        value >= min && value <= max;
}

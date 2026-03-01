// <copyright file="GameTime.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Helpers;

public static class GameTime
{
    public static int Hours(int hours)
    {
        return Math.Max(0, hours) * 60;
    }

    public static int Minutes(int minutes)
    {
        return Math.Max(0, minutes);
    }
}

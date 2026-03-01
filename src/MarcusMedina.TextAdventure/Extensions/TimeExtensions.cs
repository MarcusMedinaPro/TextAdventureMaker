// <copyright file="TimeExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Extensions;

public static class TimeExtensions
{
    public static TimeSpan Milliseconds(this int ms)
    {
        return TimeSpan.FromMilliseconds(ms);
    }

    public static TimeSpan Seconds(this int s)
    {
        return TimeSpan.FromSeconds(s);
    }

    public static TimeSpan Minutes(this int m)
    {
        return TimeSpan.FromMinutes(m);
    }
}

// <copyright file="EnumerableExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// Joins strings with ", ".
    /// </summary>
    public static string CommaJoin(this IEnumerable<string> values)
    {
        return string.Join(", ", values);
    }

    /// <summary>
    /// Joins strings with a single space.
    /// </summary>
    public static string SpaceJoin(this IEnumerable<string> values)
    {
        return string.Join(' ', values);
    }
}

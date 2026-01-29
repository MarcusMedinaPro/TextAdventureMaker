// <copyright file="EnumerableExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

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

    /// <summary>
    /// Joins entity names with ", ".
    /// </summary>
    public static string CommaJoinNames(this IEnumerable<IGameEntity> entities, bool properCase = false)
    {
        if (entities == null)
        {
            return string.Empty;
        }

        IEnumerable<string> names = entities
            .Where(entity => entity != null && !string.IsNullOrWhiteSpace(entity.Name))
            .Select(entity => entity.Name.Trim());

        if (properCase)
        {
            names = names.Select(name => name.ToProperCase());
        }

        return names.CommaJoin();
    }
}

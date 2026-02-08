// <copyright file="CollectionExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Extensions;

public static class CollectionExtensions
{
    private static readonly Random Rng = new();

    public static T PickRandom<T>(this IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);
        return list[Rng.Next(list.Count)];
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        return source.OrderBy(_ => Rng.Next());
    }

    public static T WeightedRandom<T>(this IEnumerable<T> source, Func<T, int> weightSelector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(weightSelector);

        List<T> items = source.ToList();
        int totalWeight = items.Sum(weightSelector);
        int roll = Rng.Next(totalWeight);
        int cumulative = 0;
        foreach (T item in items)
        {
            cumulative += weightSelector(item);
            if (roll < cumulative)
            {
                return item;
            }
        }

        return items.Last();
    }
}

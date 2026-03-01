// <copyright file="EventTemplates.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Template for procedurally generated events and quests.
/// </summary>
public sealed record EventTemplate(
    string Id,
    string Title,
    string Description,
    int Weight = 1
);

/// <summary>
/// Weighted table for random selection of templates.
/// </summary>
public sealed class WeightedTable<T>
{
    private readonly List<(T item, int weight)> _items = [];
    private int _totalWeight = 0;

    public void Add(T item, int weight = 1)
    {
        _items.Add((item, weight));
        _totalWeight += weight;
    }

    public T? SelectRandom(Random? rng = null)
    {
        if (_items.Count == 0)
            return default;

        rng ??= Random.Shared;
        int roll = rng.Next(_totalWeight);

        foreach (var (item, weight) in _items)
        {
            roll -= weight;
            if (roll < 0)
                return item;
        }

        return _items[^1].item;
    }

    public IEnumerable<T> SelectMultiple(int count, Random? rng = null)
    {
        for (int i = 0; i < count; i++)
        {
            var selected = SelectRandom(rng);
            if (selected != null)
                yield return selected;
        }
    }
}

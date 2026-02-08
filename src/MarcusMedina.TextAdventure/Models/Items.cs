// <copyright file="Items.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

public static class Items
{
    public static IEnumerable<Item> CreateMany(params (string id, string name, float weight)[] items)
    {
        if (items == null)
        {
            return [];
        }

        return items.Select(item => new Item(item.id, item.name).SetWeight(item.weight));
    }

    public static IEnumerable<Item> CreateMany(params (string id, string name, float weight, string desc)[] items)
    {
        if (items == null)
        {
            return [];
        }

        return items.Select(item => new Item(item.id, item.name, item.desc).SetWeight(item.weight));
    }
}

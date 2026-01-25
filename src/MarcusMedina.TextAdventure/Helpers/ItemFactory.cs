// <copyright file="ItemFactory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Helpers;

public static class ItemFactory
{
    /// <summary>
    /// Creates a basic item with a derived id, weight, and optional aliases.
    /// </summary>
    public static Item NewItem(string name, float weight = 0f, params string[] aliases)
    {
        var item = new Item(name.ToId(), name).SetWeight(weight);
        if (aliases.Length > 0)
        {
            item.AddAliases(aliases);
        }

        return item;
    }

    /// <summary>
    /// Creates a key item with a derived id, weight, and optional aliases.
    /// </summary>
    public static Key NewKey(string name, float weight = 0f, params string[] aliases)
    {
        var key = new Key(name.ToId(), name);
        key.SetWeight(weight);
        if (aliases.Length > 0)
        {
            key.AddAliases(aliases);
        }

        return key;
    }
}

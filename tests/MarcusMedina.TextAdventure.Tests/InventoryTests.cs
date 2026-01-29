// <copyright file="InventoryTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class InventoryTests
{
    [Fact]
    public void Inventory_TotalWeight_SumsItems()
    {
        Inventory inventory = new();
        _ = inventory.Add(new Item("rock", "Rock").SetWeight(1.5f));
        _ = inventory.Add(new Item("coin", "Coin").SetWeight(0.5f));

        Assert.Equal(2.0f, inventory.TotalWeight);
    }

    [Fact]
    public void Inventory_ByCount_RespectsLimit()
    {
        Inventory inventory = new(InventoryLimitType.ByCount, maxCount: 1);
        Item item1 = new("rock", "Rock");
        Item item2 = new("coin", "Coin");

        Assert.True(inventory.Add(item1));
        Assert.False(inventory.Add(item2));
    }

    [Fact]
    public void Inventory_ByWeight_RespectsLimit()
    {
        Inventory inventory = new(InventoryLimitType.ByWeight, maxWeight: 2f);
        Item item1 = new Item("rock", "Rock").SetWeight(1.5f);
        Item item2 = new Item("coin", "Coin").SetWeight(1f);

        Assert.True(inventory.Add(item1));
        Assert.False(inventory.Add(item2));
    }

    [Fact]
    public void Inventory_FindItem_UsesAliases()
    {
        Inventory inventory = new();
        Item item = new Item("sword", "Sword").AddAliases("blade");
        _ = inventory.Add(item);

        Assert.Equal(item, inventory.FindItem("blade"));
    }
}

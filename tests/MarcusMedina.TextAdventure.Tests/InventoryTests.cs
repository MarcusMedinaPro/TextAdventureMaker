// <copyright file="InventoryTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public class InventoryTests
{
    [Fact]
    public void Inventory_TotalWeight_SumsItems()
    {
        var inventory = new Inventory();
        _ = inventory.Add(new Item("rock", "Rock").SetWeight(1.5f));
        _ = inventory.Add(new Item("coin", "Coin").SetWeight(0.5f));

        Assert.Equal(2.0f, inventory.TotalWeight);
    }

    [Fact]
    public void Inventory_ByCount_RespectsLimit()
    {
        var inventory = new Inventory(InventoryLimitType.ByCount, maxCount: 1);
        var item1 = new Item("rock", "Rock");
        var item2 = new Item("coin", "Coin");

        Assert.True(inventory.Add(item1));
        Assert.False(inventory.Add(item2));
    }

    [Fact]
    public void Inventory_ByWeight_RespectsLimit()
    {
        var inventory = new Inventory(InventoryLimitType.ByWeight, maxWeight: 2f);
        var item1 = new Item("rock", "Rock").SetWeight(1.5f);
        var item2 = new Item("coin", "Coin").SetWeight(1f);

        Assert.True(inventory.Add(item1));
        Assert.False(inventory.Add(item2));
    }

    [Fact]
    public void Inventory_FindItem_UsesAliases()
    {
        var inventory = new Inventory();
        var item = new Item("sword", "Sword").AddAliases("blade");
        _ = inventory.Add(item);

        Assert.Equal(item, inventory.FindItem("blade"));
    }
}

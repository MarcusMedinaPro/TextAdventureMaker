// <copyright file="ContainerTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class ContainerTests
{
    [Fact]
    public void Glass_HoldsFluids()
    {
        Glass glass = new("glass", "glass");
        IFluid water = new Fluid("water", "water");

        Assert.True(glass.Add(water));
        _ = Assert.Single(glass.Contents);
    }

    [Fact]
    public void Chest_HoldsItems()
    {
        Chest chest = new("chest", "chest");
        Item coin = new("coin", "coin");

        Assert.True(chest.Add(coin));
        _ = Assert.Single(chest.Contents);
    }

    [Fact]
    public void Container_RespectsMaxCount()
    {
        Chest chest = new("chest", "chest", maxCount: 1);
        Assert.True(chest.Add(new Item("coin", "coin")));
        Assert.False(chest.Add(new Item("gem", "gem")));
    }
}

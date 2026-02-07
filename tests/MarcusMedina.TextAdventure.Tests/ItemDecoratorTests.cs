// <copyright file="ItemDecoratorTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class ItemDecoratorTests
{
    [Fact]
    public void RustyModifier_PrefixesNameAndDescription()
    {
        IItem item = new Item("sword", "sword").Description("A sharp blade.");
        RustyModifier rusty = new(item);

        Assert.Equal("rusty sword", rusty.Name);
        Assert.Contains("rusty", rusty.GetDescription(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EnchantedModifier_PrefixesNameAndDescription()
    {
        IItem item = new Item("ring", "ring").Description("A silver ring.");
        EnchantedModifier enchanted = new(item);

        Assert.Equal("enchanted ring", enchanted.Name);
        Assert.Contains("magical", enchanted.GetDescription(), StringComparison.OrdinalIgnoreCase);
    }
}

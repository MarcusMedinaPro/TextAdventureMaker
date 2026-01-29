// <copyright file="ItemDecoratorTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Models;

public class ItemDecoratorTests
{
    [Fact]
    public void RustyModifier_PrefixesNameAndDescription()
    {
        var item = new Item("sword", "sword").Description("A sharp blade.");
        var rusty = new RustyModifier(item);

        Assert.Equal("rusty sword", rusty.Name);
        Assert.Contains("rusty", rusty.GetDescription(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EnchantedModifier_PrefixesNameAndDescription()
    {
        var item = new Item("ring", "ring").Description("A silver ring.");
        var enchanted = new EnchantedModifier(item);

        Assert.Equal("enchanted ring", enchanted.Name);
        Assert.Contains("magical", enchanted.GetDescription(), StringComparison.OrdinalIgnoreCase);
    }
}

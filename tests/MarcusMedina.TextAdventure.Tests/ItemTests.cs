// <copyright file="ItemTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

public class ItemTests
{
    [Fact]
    public void Item_Matches_Name_And_Alias()
    {
        var item = new Item("sword", "Sword").AddAliases("blade", "steel");

        Assert.True(item.Matches("sword"));
        Assert.True(item.Matches("Blade"));
        Assert.False(item.Matches("hammer"));
    }

    [Fact]
    public void Item_InvalidIdOrName_Throws()
    {
        _ = Assert.Throws<ArgumentException>(() => new Item("", "Item"));
        _ = Assert.Throws<ArgumentException>(() => new Item("id", ""));
        _ = Assert.Throws<ArgumentNullException>(() => new Item(null!, "Item"));
        _ = Assert.Throws<ArgumentNullException>(() => new Item("id", null!));
    }

    [Fact]
    public void Item_CanHaveDescription()
    {
        var item = new Item("coin", "Coin")
            .Description("A shiny gold coin.");

        Assert.Equal("A shiny gold coin.", item.GetDescription());
    }

    [Fact]
    public void Key_CanHaveDescription()
    {
        var key = new Key("key1", "Brass Key")
            .Description("A small brass key with a lion crest.");

        Assert.Equal("A small brass key with a lion crest.", key.GetDescription());
    }

    [Fact]
    public void Item_CanUseTupleConstructor()
    {
        Item torch = (id: "torch", name: "Torch", description: "A smoky torch.");

        Assert.Equal("Torch", torch.Name);
        Assert.Equal("A smoky torch.", torch.GetDescription());
    }

    [Fact]
    public void ItemFactory_DerivesIdFromName()
    {
        var item = ItemFactory.NewItem("Red Apple", 0.5f);

        Assert.Equal("red_apple", item.Id);
        Assert.Equal(0.5f, item.Weight);
    }

    [Fact]
    public void Item_Interface_AllowsFluentAliases()
    {
        var item = new Item("coin", "Coin")
            .AddAliases("token")
            .SetReaction(ItemAction.Take, "You pick it up.");

        Assert.True(item.Matches("token"));
    }

    [Fact]
    public void EnumerableExtensions_CanJoinEntityNames()
    {
        var items = new IItem[]
        {
            new Item("coin", "gold coin"),
            new Item("torch", "old torch")
        };

        var joined = items.CommaJoinNames(properCase: true);

        Assert.Equal("Gold Coin, Old Torch", joined);
    }

    [Fact]
    public void Item_Clone_CopiesConfig()
    {
        var original = new Item("note", "Note")
            .AddAliases("paper")
            .SetTakeable(false)
            .SetWeight(0.5f)
            .SetReadable(true)
            .SetReadText("Hello")
            .SetReaction(ItemAction.Use, "Used.");

        _ = original.SetProperty("mood", "quiet");

        var clone = original.Clone();

        Assert.NotSame(original, clone);
        Assert.Equal(original.Id, clone.Id);
        Assert.Equal(original.Name, clone.Name);
        Assert.Equal(original.GetDescription(), clone.GetDescription());
        Assert.Equal(original.Takeable, clone.Takeable);
        Assert.Equal(original.Weight, clone.Weight);
        Assert.Equal(original.Readable, clone.Readable);
        Assert.Equal(original.GetReadText(), clone.GetReadText());
        Assert.Equal(original.GetReaction(ItemAction.Use), clone.GetReaction(ItemAction.Use));
        Assert.Equal(original.GetProperty("mood"), clone.GetProperty("mood"));
    }
}

// <copyright file="GameItemListTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class GameItemListTests
{
    [Fact]
    public void AddMany_CreatesItemsWithIdsFromNames()
    {
        GameItemList list = new GameItemList()
            .AddMany("rubber chicken", "sword");

        Item chicken = list.Get("rubber chicken");
        Item sword = list.Get("sword");

        Assert.Equal("rubber_chicken", chicken.Id);
        Assert.Equal("sword", sword.Id);
    }

    [Fact]
    public void Find_MatchesAliases()
    {
        GameItemList list = new();
        _ = list.Add("cat").AddAliases("kitten", "kitteh");

        Item? found = list.Find("kitteh");

        Assert.NotNull(found);
        Assert.Equal("cat", found!.Name);
    }

    [Fact]
    public void Indexer_ReturnsItem()
    {
        GameItemList list = new GameItemList().AddMany("cat");

        Item item = list["cat"];

        Assert.Equal("cat", item.Name);
    }

    [Fact]
    public void Call_ReturnsItem()
    {
        GameItemList list = new GameItemList().AddMany("cat");

        Item item = list.Call("cat");

        Assert.Equal("cat", item.Name);
    }
}

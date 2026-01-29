// <copyright file="GameItemListTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Models;

public class GameItemListTests
{
    [Fact]
    public void AddMany_CreatesItemsWithIdsFromNames()
    {
        var list = new GameItemList()
            .AddMany("rubber chicken", "sword");

        var chicken = list.Get("rubber chicken");
        var sword = list.Get("sword");

        Assert.Equal("rubber_chicken", chicken.Id);
        Assert.Equal("sword", sword.Id);
    }

    [Fact]
    public void Find_MatchesAliases()
    {
        var list = new GameItemList();
        _ = list.Add("cat").AddAliases("kitten", "kitteh");

        var found = list.Find("kitteh");

        Assert.NotNull(found);
        Assert.Equal("cat", found!.Name);
    }

    [Fact]
    public void Indexer_ReturnsItem()
    {
        var list = new GameItemList().AddMany("cat");

        var item = list["cat"];

        Assert.Equal("cat", item.Name);
    }

    [Fact]
    public void Call_ReturnsItem()
    {
        var list = new GameItemList().AddMany("cat");

        var item = list.Call("cat");

        Assert.Equal("cat", item.Name);
    }
}

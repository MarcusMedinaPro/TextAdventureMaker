// <copyright file="GameListTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class GameListTests
{
    [Fact]
    public void AddMany_CreatesItemsWithFactory()
    {
        GameList<Item> list = new(name => new Item(name.ToId(), name));
        _ = list.AddMany("cat", "rubber chicken");

        Assert.Equal("cat", list["cat"].Name);
        Assert.Equal("rubber_chicken", list["rubber chicken"].Id);
    }

    [Fact]
    public void Find_UsesItemAliasesWhenAvailable()
    {
        GameList<Item> list = new(name => new Item(name.ToId(), name));
        _ = list.Add("cat").AddAliases("kitten");

        Item? found = list.Find("kitten");

        Assert.NotNull(found);
        Assert.Equal("cat", found!.Name);
    }

    [Fact]
    public void Find_MatchesByIdOrNameForDoors()
    {
        GameList<Door> list = new(name => new Door(name.ToId(), name));
        _ = list.AddMany("shed door");

        Assert.NotNull(list.Find("shed_door"));
        Assert.NotNull(list.Find("shed door"));
    }

    [Fact]
    public void WrapperLists_UseDefaultFactories()
    {
        ItemList items = new ItemList().AddMany("cat");
        KeyList keys = new KeyList().AddMany("shed key");
        DoorList doors = new DoorList().AddMany("shed door");
        NpcList npcs = new NpcList().AddMany("fox");
        LocationList locations = new LocationList().AddMany("forest");

        Assert.Equal("cat", items["cat"].Name);
        Assert.Equal("shed_key", keys["shed key"].Id);
        Assert.Equal("shed_door", doors["shed door"].Id);
        Assert.Equal("fox", npcs["fox"].Name);
        Assert.Equal("forest", locations["forest"].Id);
    }
}

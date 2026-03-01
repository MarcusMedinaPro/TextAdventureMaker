// <copyright file="Slice047StackableItemsTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

public class Slice047StackableItemsTests
{
    [Fact]
    public void Inventory_Add_StackableSameId_MergesAmounts()
    {
        Inventory inventory = new();
        Item first = new("arrow", "Arrow");
        Item second = new("arrow", "Arrow");
        _ = first.SetStackable().SetAmount(2);
        _ = second.SetStackable().SetAmount(3);

        _ = inventory.Add(first);
        _ = inventory.Add(second);

        IItem merged = Assert.Single(inventory.Items);
        Assert.Equal(5, merged.Amount);
    }

    [Fact]
    public void Inventory_Add_StackableDifferentIds_KeepsSeparateEntries()
    {
        Inventory inventory = new();
        Item arrows = new("arrow", "Arrow");
        Item bolts = new("bolt", "Bolt");
        _ = arrows.SetStackable().SetAmount(2);
        _ = bolts.SetStackable().SetAmount(3);

        _ = inventory.Add(arrows);
        _ = inventory.Add(bolts);

        Assert.Equal(2, inventory.Count);
    }

    [Fact]
    public void Inventory_FindById_IsCaseInsensitive()
    {
        Inventory inventory = new();
        Item arrows = new("arrow", "Arrow");
        _ = arrows.SetStackable().SetAmount(2);
        _ = inventory.Add(arrows);

        IItem? found = inventory.FindById("ARROW");

        Assert.NotNull(found);
        Assert.Equal("arrow", found.Id);
    }

    [Fact]
    public void Inventory_TotalWeight_UsesStackAmount()
    {
        Inventory inventory = new();
        Item arrows = new("arrow", "Arrow");
        _ = arrows.SetStackable().SetAmount(8).SetWeight(0.1f);
        _ = inventory.Add(arrows);

        Assert.Equal(0.8f, inventory.TotalWeight, 2);
    }

    [Fact]
    public void TakeCommand_PartialTake_SplitsRoomStackAndAddsInventory()
    {
        Location room = new("room");
        Item arrows = new("arrow", "Arrow");
        _ = arrows.SetStackable().SetAmount(10);
        room.AddItem(arrows);
        GameState state = new(room);

        CommandResult result = state.Execute(new TakeCommand("arrow", 3));

        Assert.True(result.Success);
        Assert.Equal(7, room.FindItem("arrow")?.Amount);
        Assert.Equal(3, state.Inventory.FindById("arrow")?.Amount);
    }

    [Fact]
    public void TakeCommand_FullStackTake_RemovesRoomItem()
    {
        Location room = new("room");
        Item arrows = new("arrow", "Arrow");
        _ = arrows.SetStackable().SetAmount(10);
        room.AddItem(arrows);
        GameState state = new(room);

        CommandResult result = state.Execute(new TakeCommand("arrow", 10));

        Assert.True(result.Success);
        Assert.Null(room.FindItem("arrow"));
        Assert.Equal(10, state.Inventory.FindById("arrow")?.Amount);
    }

    [Fact]
    public void DropCommand_PartialDrop_ReducesInventoryAndAddsRoomStack()
    {
        Location room = new("room");
        GameState state = new(room);
        Item arrows = new("arrow", "Arrow");
        _ = arrows.SetStackable().SetAmount(8);
        _ = state.Inventory.Add(arrows);

        CommandResult result = state.Execute(new DropCommand("arrow", 3));

        Assert.True(result.Success);
        Assert.Equal(5, state.Inventory.FindById("arrow")?.Amount);
        Assert.Equal(3, room.FindItem("arrow")?.Amount);
    }

    [Fact]
    public void DropCommand_FullDrop_RemovesFromInventory()
    {
        Location room = new("room");
        GameState state = new(room);
        Item arrows = new("arrow", "Arrow");
        _ = arrows.SetStackable().SetAmount(4);
        _ = state.Inventory.Add(arrows);

        CommandResult result = state.Execute(new DropCommand("arrow", 4));

        Assert.True(result.Success);
        Assert.Null(state.Inventory.FindById("arrow"));
        Assert.Equal(4, room.FindItem("arrow")?.Amount);
    }

    [Fact]
    public void InventoryCommand_ShowsGroupedStackAmount()
    {
        Location room = new("room");
        GameState state = new(room);
        Item arrows = new("arrow", "Arrow");
        _ = arrows.SetStackable().SetAmount(8);
        _ = state.Inventory.Add(arrows);

        CommandResult result = state.Execute(new InventoryCommand());

        Assert.True(result.Success);
        Assert.Contains("(x8)", result.Message);
    }

    [Fact]
    public void TakeAllCommand_StackItems_MergesIntoExistingInventoryStack()
    {
        Location room = new("room");
        Item roomArrows = new("arrow", "Arrow");
        _ = roomArrows.SetStackable().SetAmount(3);
        room.AddItem(roomArrows);
        room.AddItem(new Item("apple", "Apple"));
        GameState state = new(room);

        Item existing = new("arrow", "Arrow");
        _ = existing.SetStackable().SetAmount(2);
        _ = state.Inventory.Add(existing);

        CommandResult result = state.Execute(new TakeAllCommand());

        Assert.True(result.Success);
        Assert.Equal(5, state.Inventory.FindById("arrow")?.Amount);
        Assert.Equal(2, state.Inventory.Count);
        Assert.Empty(room.Items);
    }

    [Fact]
    public void DropAllCommand_DropsStackableAndNonStackable()
    {
        Location room = new("room");
        GameState state = new(room);
        Item arrows = new("arrow", "Arrow");
        _ = arrows.SetStackable().SetAmount(5);
        _ = state.Inventory.Add(arrows);
        _ = state.Inventory.Add(new Item("coin", "Coin"));

        CommandResult result = state.Execute(new DropAllCommand());

        Assert.True(result.Success);
        Assert.Equal(0, state.Inventory.Count);
        Assert.Equal(2, room.Items.Count);
        Assert.Equal(5, room.FindItem("arrow")?.Amount);
    }

    [Fact]
    public void KeywordParser_ParseTakeWithAmount_ReturnsTakeCommandWithAmount()
    {
        KeywordParser parser = new(KeywordParserConfigBuilder.BritishDefaults().Build());

        ICommand command = parser.Parse("take 3 arrows");

        TakeCommand take = Assert.IsType<TakeCommand>(command);
        Assert.Equal(3, take.Amount);
        Assert.Equal("arrows", take.ItemName);
    }

    [Fact]
    public void KeywordParser_ParseDropWithAmount_ReturnsDropCommandWithAmount()
    {
        KeywordParser parser = new(KeywordParserConfigBuilder.BritishDefaults().Build());

        ICommand command = parser.Parse("drop 2 arrows");

        DropCommand drop = Assert.IsType<DropCommand>(command);
        Assert.Equal(2, drop.Amount);
        Assert.Equal("arrows", drop.ItemName);
    }

    [Fact]
    public void StackExtensions_SplitStack_ProducesSplitAndRemainder()
    {
        Item arrows = new("arrow", "Arrow");
        _ = arrows.SetStackable().SetAmount(10);

        IItem? split = arrows.SplitStack(3);

        Assert.NotNull(split);
        Assert.Equal(3, split.Amount);
        Assert.Equal(7, arrows.Amount);
    }

    [Fact]
    public void StackExtensions_TryMerge_MergesMatchingStacks()
    {
        Item target = new("arrow", "Arrow");
        Item source = new("arrow", "Arrow");
        _ = target.SetStackable().SetAmount(4);
        _ = source.SetStackable().SetAmount(5);

        bool merged = target.TryMerge(source);

        Assert.True(merged);
        Assert.Equal(9, target.Amount);
    }
}

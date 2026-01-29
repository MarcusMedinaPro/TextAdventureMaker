// <copyright file="ItemCommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class ItemCommandTests
{
    [Fact]
    public void TakeCommand_MovesItemToInventory()
    {
        Location location = new("room");
        Item item = new Item("apple", "Apple").SetWeight(1f);
        location.AddItem(item);
        GameState state = new(location);

        CommandResult result = state.Execute(new TakeCommand("apple"));

        Assert.True(result.Success);
        Assert.Empty(location.Items);
        _ = Assert.Single(state.Inventory.Items);
    }

    [Fact]
    public void TakeCommand_FailsWhenNotTakeable()
    {
        Location location = new("room");
        Item item = new Item("statue", "Statue").SetTakeable(false);
        location.AddItem(item);
        GameState state = new(location);

        CommandResult result = state.Execute(new TakeCommand("statue"));

        Assert.False(result.Success);
        Assert.Equal(Language.CannotTakeItem, result.Message);
    }

    [Fact]
    public void DropCommand_MovesItemToLocation()
    {
        Location location = new("room");
        GameState state = new(location);
        Item item = new("coin", "Coin");
        _ = state.Inventory.Add(item);

        CommandResult result = state.Execute(new DropCommand("coin"));

        Assert.True(result.Success);
        _ = Assert.Single(location.Items);
        Assert.Empty(state.Inventory.Items);
    }

    [Fact]
    public void InventoryCommand_ShowsTotalWeight()
    {
        Location location = new("room");
        GameState state = new(location);
        _ = state.Inventory.Add(new Item("rock", "Rock").SetWeight(2f));

        CommandResult result = state.InventoryView();

        Assert.Contains(Language.TotalWeight(2f), result.Message);
    }

    [Fact]
    public void UseCommand_RequiresItemInInventory()
    {
        Location location = new("room");
        GameState state = new(location);

        CommandResult result = state.Execute(new UseCommand("wand"));

        Assert.False(result.Success);
        Assert.Equal(Language.NoSuchItemInventory, result.Message);
    }

    [Fact]
    public void TakeAllCommand_TakesAllTakeableItems()
    {
        Location location = new("room");
        location.AddItem(new Item("coin", "Coin"));
        location.AddItem(new Item("apple", "Apple"));
        GameState state = new(location);

        CommandResult result = state.Execute(new TakeAllCommand());

        Assert.True(result.Success);
        Assert.Empty(location.Items);
        Assert.Equal(2, state.Inventory.Count);
    }

    [Fact]
    public void TakeAllCommand_SkipsTooHeavyItems()
    {
        Location location = new("room");
        location.AddItem(new Item("rock", "Rock").SetWeight(5f));
        Inventory inventory = new(InventoryLimitType.ByWeight, maxWeight: 1f);
        GameState state = new(location, inventory: inventory);

        CommandResult result = state.Execute(new TakeAllCommand());

        Assert.False(result.Success);
        Assert.Equal(Language.TooHeavy, result.Message);
    }

    [Fact]
    public void DropAllCommand_DropsEverything()
    {
        Location location = new("room");
        GameState state = new(location);
        _ = state.Inventory.Add(new Item("coin", "Coin"));
        _ = state.Inventory.Add(new Item("apple", "Apple"));

        CommandResult result = state.Execute(new DropAllCommand());

        Assert.True(result.Success);
        Assert.Equal(0, state.Inventory.Count);
        Assert.Equal(2, location.Items.Count);
    }
}

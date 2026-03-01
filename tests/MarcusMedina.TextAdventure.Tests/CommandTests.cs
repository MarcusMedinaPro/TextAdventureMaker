// <copyright file="CommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

public class CommandTests
{
    [Fact]
    public void CommandContext_NullState_Throws() => _ = Assert.Throws<ArgumentNullException>(() => new CommandContext(null!));

    [Fact]
    public void GoCommand_MovesWhenExitExists()
    {
        Location start = new("start");
        Location next = new("next");
        _ = start.AddExit(Direction.North, next);
        GameState state = new(start);

        var result = state.Execute(new GoCommand(Direction.North));

        Assert.True(result.Success);
        Assert.Equal(next, state.CurrentLocation);
    }

    [Fact]
    public void GoCommand_ReturnsErrorWhenNoExit()
    {
        GameState state = new(new Location("start"));

        var result = state.Execute(new GoCommand(Direction.North));

        Assert.False(result.Success);
        Assert.Equal(Language.CantGoThatWay, result.Message);
    }

    [Fact]
    public void GoToCommand_GoesThroughDoor()
    {
        Location start = new("start");
        Location next = new("next");
        Door door = new("door1", "iron door");
        _ = door.Open();
        _ = start.AddExit(Direction.North, next, door);
        GameState state = new(start);

        var result = state.Execute(new GoToCommand("door"));

        Assert.True(result.Success);
        Assert.Equal(next, state.CurrentLocation);
    }

    [Fact]
    public void LookCommand_CanLookAtDoor()
    {
        Location start = new("start");
        Location next = new("next");
        var door = new Door("door1", "oak door").Description("An old oak door.");
        _ = start.AddExit(Direction.North, next, door);
        GameState state = new(start);

        var result = state.Execute(new LookCommand("door"));

        Assert.True(result.Success);
        Assert.Equal("An old oak door.", result.Message);
    }

    [Fact]
    public void LookCommand_CanLookAtItemInInventory()
    {
        Location start = new("start");
        var coin = new Item("coin", "Coin").Description("A shiny coin.");
        GameState state = new(start);
        _ = state.Inventory.Add(coin);

        var result = state.Execute(new LookCommand("coin"));

        Assert.True(result.Success);
        Assert.Equal("A shiny coin.", result.Message);
    }

    [Fact]
    public void LookCommand_CanLookAtItemInLocation()
    {
        Location start = new("start");
        var key = new Key("key1", "Brass Key")
            .AddAliases("key")
            .Description("A small brass key.");
        start.AddItem(key);
        GameState state = new(start);

        var result = state.Execute(new LookCommand("key"));

        Assert.True(result.Success);
        Assert.Equal("A small brass key.", result.Message);
    }

    [Fact]
    public void LookCommand_CanLookAtLocation()
    {
        var start = new Location("start").Description("A quiet room.");
        GameState state = new(start);

        var result = state.Execute(new LookCommand("room"));

        Assert.True(result.Success);
        Assert.Equal("A quiet room.", result.Message);
    }

    [Fact]
    public void LookCommand_ListsDescriptionAndExits()
    {
        var start = new Location("start").Description("A small room.");
        Location next = new("next");
        _ = start.AddExit(Direction.East, next);
        GameState state = new(start);

        var result = state.Look();

        Assert.True(result.Success);
        Assert.Contains("A small room.", result.Message);
        Assert.Contains(Language.HealthStatus(100, 100), result.Message);
        Assert.Contains(Language.ItemsHereLabel, result.Message);
        Assert.Contains("Exits:", result.Message);
        Assert.Contains("East", result.Message);
    }

    [Fact]
    public void OpenCommand_FailsWhenLocked()
    {
        Location start = new("start");
        Location next = new("next");
        var door = new Door("door1", "iron door").RequiresKey(new Key("key1", "key"));
        _ = start.AddExit(Direction.East, next, door);
        GameState state = new(start);

        var result = state.Execute(new OpenCommand());

        Assert.False(result.Success);
        Assert.Equal(Language.DoorLocked("iron door"), result.Message);
    }

    [Fact]
    public void OpenCommand_FailsWhenNoDoor()
    {
        Location start = new("start");
        Location next = new("next");
        _ = start.AddExit(Direction.East, next);
        GameState state = new(start);

        var result = state.Execute(new OpenCommand());

        Assert.False(result.Success);
        Assert.Equal(Language.NoDoorHere, result.Message);
    }

    [Fact]
    public void StatsCommand_ReturnsHealthStatus()
    {
        Stats stats = new(20, currentHealth: 7);
        GameState state = new(new Location("start"), stats);

        var result = state.StatsView();

        Assert.True(result.Success);
        Assert.Equal(Language.HealthStatus(7, 20), result.Message);
    }

    [Fact]
    public void UnlockCommand_FailsWhenNoKey()
    {
        Location start = new("start");
        Location next = new("next");
        var door = new Door("door1", "iron door").RequiresKey(new Key("key1", "key"));
        _ = start.AddExit(Direction.East, next, door);
        GameState state = new(start);

        var result = state.Execute(new UnlockCommand());

        Assert.False(result.Success);
        Assert.Equal(Language.YouNeedAKeyToOpenDoor, result.Message);
    }

    [Fact]
    public void UnlockCommand_SucceedsWithCorrectKey()
    {
        Key key = new("key1", "key");
        Location start = new("start");
        Location next = new("next");
        var door = new Door("door1", "iron door").RequiresKey(key);
        _ = start.AddExit(Direction.East, next, door);
        GameState state = new(start);

        _ = state.Inventory.Add(key);
        var result = state.Execute(new UnlockCommand());

        Assert.True(result.Success);
        Assert.Equal(Language.DoorUnlocked("iron door"), result.Message);
    }
}
// <copyright file="GameErrorTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class GameErrorTests
{
    [Fact]
    public void GoCommand_NoExit_ReturnsNoExitError()
    {
        GameState state = new(new Location("start"));

        CommandResult result = state.Execute(new GoCommand(Direction.North));

        Assert.False(result.Success);
        Assert.Equal(GameError.NoExitInDirection, result.Error);
    }

    [Fact]
    public void GoCommand_LockedDoor_ReturnsDoorLockedError()
    {
        Location start = new("start");
        Location next = new("next");
        Door door = new("door1", "gate", DoorState.Locked);
        _ = start.AddExit(Direction.North, next, door);
        GameState state = new(start);

        CommandResult result = state.Execute(new GoCommand(Direction.North));

        Assert.False(result.Success);
        Assert.Equal(GameError.DoorIsLocked, result.Error);
    }

    [Fact]
    public void OpenCommand_NoDoor_ReturnsNoDoorError()
    {
        Location start = new("start");
        Location next = new("next");
        _ = start.AddExit(Direction.East, next);
        GameState state = new(start);

        CommandResult result = state.Execute(new OpenCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.NoDoorHere, result.Error);
    }

    [Fact]
    public void OpenCommand_LockedDoor_ReturnsDoorLockedError()
    {
        Location start = new("start");
        Location next = new("next");
        Door door = new("door1", "iron door", DoorState.Locked);
        _ = start.AddExit(Direction.East, next, door);
        GameState state = new(start);

        CommandResult result = state.Execute(new OpenCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.DoorIsLocked, result.Error);
    }

    [Fact]
    public void OpenCommand_AlreadyOpen_ReturnsDoorAlreadyOpenError()
    {
        Location start = new("start");
        Location next = new("next");
        Door door = new("door1", "iron door", DoorState.Open);
        _ = start.AddExit(Direction.East, next, door);
        GameState state = new(start);

        CommandResult result = state.Execute(new OpenCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.DoorAlreadyOpen, result.Error);
    }

    [Fact]
    public void UnlockCommand_NoKeyRequired_ReturnsNoKeyRequiredError()
    {
        Location start = new("start");
        Location next = new("next");
        Door door = new("door1", "iron door", DoorState.Closed);
        _ = start.AddExit(Direction.East, next, door);
        GameState state = new(start);

        CommandResult result = state.Execute(new UnlockCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.NoKeyRequired, result.Error);
    }

    [Fact]
    public void UnlockCommand_WrongKey_ReturnsWrongKeyError()
    {
        Key correct = new("key1", "key");
        Key wrong = new("key2", "wrong");
        Location start = new("start");
        Location next = new("next");
        Door door = new Door("door1", "iron door").RequiresKey(correct);
        _ = start.AddExit(Direction.East, next, door);
        GameState state = new(start);
        _ = state.Inventory.Add(wrong);

        CommandResult result = state.Execute(new UnlockCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.WrongKey, result.Error);
    }

    [Fact]
    public void TakeCommand_ItemNotFound_ReturnsItemNotFoundError()
    {
        Location start = new("start");
        GameState state = new(start);

        CommandResult result = state.Execute(new TakeCommand("coin"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
    }

    [Fact]
    public void TakeCommand_InventoryFull_ReturnsInventoryFullError()
    {
        Location start = new("start");
        Item item = new("coin", "Coin");
        start.AddItem(item);
        Inventory inventory = new(InventoryLimitType.ByCount, maxCount: 0);
        GameState state = new(start, inventory: inventory);

        CommandResult result = state.Execute(new TakeCommand("coin"));

        Assert.False(result.Success);
        Assert.Equal(GameError.InventoryFull, result.Error);
    }

    [Fact]
    public void TakeAllCommand_NoItems_ReturnsItemNotFoundError()
    {
        Location start = new("start");
        GameState state = new(start);

        CommandResult result = state.Execute(new TakeAllCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
    }

    [Fact]
    public void TakeAllCommand_InventoryFull_ReturnsInventoryFullError()
    {
        Location start = new("start");
        start.AddItem(new Item("coin", "Coin"));
        Inventory inventory = new(InventoryLimitType.ByCount, maxCount: 0);
        GameState state = new(start, inventory: inventory);

        CommandResult result = state.Execute(new TakeAllCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.InventoryFull, result.Error);
    }

    [Fact]
    public void DropCommand_ItemNotInInventory_ReturnsItemNotInInventoryError()
    {
        Location start = new("start");
        GameState state = new(start);

        CommandResult result = state.Execute(new DropCommand("coin"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotInInventory, result.Error);
    }

    [Fact]
    public void DropAllCommand_EmptyInventory_ReturnsItemNotInInventoryError()
    {
        Location start = new("start");
        GameState state = new(start);

        CommandResult result = state.Execute(new DropAllCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotInInventory, result.Error);
    }

    [Fact]
    public void UseCommand_ItemNotFound_ReturnsItemNotFoundError()
    {
        Location start = new("start");
        GameState state = new(start);

        CommandResult result = state.Execute(new UseCommand("wand"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
    }
}

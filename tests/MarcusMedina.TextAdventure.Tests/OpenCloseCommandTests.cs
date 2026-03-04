// <copyright file="OpenCloseCommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class OpenCloseCommandTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static (GameState state, Door door) RoomWithDoor(string doorId = "gate", string doorName = "iron gate")
    {
        var room = new Location("hall");
        var next = new Location("yard");
        var door = new Door(doorId, doorName);
        room.AddExit(Direction.North, next, door);
        return (new GameState(room), door);
    }

    private static (GameState state, ContainerItem<IItem> chest) RoomWithChest(string chestId = "chest", string chestName = "wooden chest")
    {
        var room = new Location("hall");
        var chest = new ContainerItem<IItem>(chestId, chestName);
        room.AddItem(chest);
        return (new GameState(room), chest);
    }

    // ── OpenCommand: door targeting ──────────────────────────────────────────

    [Fact]
    public void OpenCommand_OpensNamedDoor()
    {
        var (state, door) = RoomWithDoor("gate", "iron gate");
        var result = state.Execute(new OpenCommand("iron gate"));
        Assert.True(result.Success);
        Assert.Equal(DoorState.Open, door.State);
    }

    [Fact]
    public void OpenCommand_OpensFirstDoorWithNoTarget()
    {
        var (state, door) = RoomWithDoor();
        var result = state.Execute(new OpenCommand(null));
        Assert.True(result.Success);
        Assert.Equal(DoorState.Open, door.State);
    }

    [Fact]
    public void OpenCommand_FailsOnWrongDoorName()
    {
        var (state, _) = RoomWithDoor("gate", "iron gate");
        var result = state.Execute(new OpenCommand("wooden door"));
        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
    }

    [Fact]
    public void OpenCommand_SelectsCorrectDoorWhenMultipleExist()
    {
        var room = new Location("hall");
        var d1 = new Door("d1", "oak door");
        var d2 = new Door("d2", "iron gate");
        room.AddExit(Direction.North, new Location("n"), d1);
        room.AddExit(Direction.South, new Location("s"), d2);
        var state = new GameState(room);

        var result = state.Execute(new OpenCommand("iron gate"));
        Assert.True(result.Success);
        Assert.Equal(DoorState.Open, d2.State);
        Assert.NotEqual(DoorState.Open, d1.State);
    }

    // ── OpenCommand: container support ──────────────────────────────────────

    [Fact]
    public void OpenCommand_OpensChestByName()
    {
        var (state, chest) = RoomWithChest();
        var result = state.Execute(new OpenCommand("wooden chest"));
        Assert.True(result.Success);
    }

    [Fact]
    public void OpenCommand_ListsContainerContents()
    {
        var (state, chest) = RoomWithChest();
        var coin = new Item("coin", "gold coin");
        chest.Add(coin);

        var result = state.Execute(new OpenCommand("wooden chest"));
        Assert.True(result.Success);
        Assert.Contains("gold coin", result.Message);
    }

    [Fact]
    public void OpenCommand_EmptyContainerSaysEmpty()
    {
        var (state, _) = RoomWithChest();
        var result = state.Execute(new OpenCommand("wooden chest"));
        Assert.True(result.Success);
        Assert.Contains(Language.ContainerIsEmpty, result.Message);
    }

    [Fact]
    public void OpenCommand_FallsBackToContainerWithNoTarget()
    {
        var (state, _) = RoomWithChest();
        var result = state.Execute(new OpenCommand(null));
        Assert.True(result.Success);
    }

    [Fact]
    public void OpenCommand_FailsOnNonOpenableItem()
    {
        var room = new Location("hall");
        room.AddItem(new Item("rock", "smooth rock"));
        var state = new GameState(room);

        var result = state.Execute(new OpenCommand("smooth rock"));
        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotUsable, result.Error);
    }

    [Fact]
    public void OpenCommand_FailsWhenNothingToOpen()
    {
        var room = new Location("hall");
        var state = new GameState(room);

        var result = state.Execute(new OpenCommand(null));
        Assert.False(result.Success);
        Assert.Equal(Language.NothingToOpen, result.Message);
    }

    // ── CloseCommand: door targeting ─────────────────────────────────────────

    [Fact]
    public void CloseCommand_ClosesNamedDoor()
    {
        var (state, door) = RoomWithDoor("gate", "iron gate");
        door.Open();
        var result = state.Execute(new CloseCommand("iron gate"));
        Assert.True(result.Success);
        Assert.Equal(DoorState.Closed, door.State);
    }

    [Fact]
    public void CloseCommand_FailsOnWrongDoorName()
    {
        var (state, door) = RoomWithDoor("gate", "iron gate");
        door.Open();
        var result = state.Execute(new CloseCommand("wooden door"));
        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
    }

    // ── CloseCommand: container support ─────────────────────────────────────

    [Fact]
    public void CloseCommand_ClosesContainerByName()
    {
        var (state, chest) = RoomWithChest();
        var result = state.Execute(new CloseCommand("wooden chest"));
        Assert.True(result.Success);
        Assert.Contains("wooden chest", result.Message);
    }

    // ── LockCommand: named door ──────────────────────────────────────────────

    [Fact]
    public void LockCommand_LocksNamedDoor()
    {
        var key = new Key("key1", "brass key");
        var room = new Location("hall");
        var door = new Door("gate", "iron gate").RequiresKey(key);
        room.AddExit(Direction.North, new Location("yard"), door);
        var state = new GameState(room);
        state.Inventory.Add(key);
        door.Unlock(key); // door starts locked when key is required; unlock first

        var result = state.Execute(new LockCommand("iron gate"));
        Assert.True(result.Success);
        Assert.Equal(DoorState.Locked, door.State);
    }

    [Fact]
    public void LockCommand_FailsOnWrongDoorName()
    {
        var key = new Key("key1", "brass key");
        var room = new Location("hall");
        var door = new Door("gate", "iron gate").RequiresKey(key);
        room.AddExit(Direction.North, new Location("yard"), door);
        var state = new GameState(room);
        state.Inventory.Add(key);

        var result = state.Execute(new LockCommand("oak door"));
        Assert.False(result.Success);
        Assert.Equal(GameError.NoDoorHere, result.Error);
    }

    // ── UnlockCommand: named door ────────────────────────────────────────────

    [Fact]
    public void UnlockCommand_UnlocksNamedDoor()
    {
        var key = new Key("key1", "brass key");
        var room = new Location("hall");
        var door = new Door("gate", "iron gate").RequiresKey(key);
        room.AddExit(Direction.North, new Location("yard"), door);
        var state = new GameState(room);
        state.Inventory.Add(key);

        var result = state.Execute(new UnlockCommand("iron gate"));
        Assert.True(result.Success);
        Assert.Equal(DoorState.Closed, door.State);
    }

    [Fact]
    public void UnlockCommand_FailsOnWrongDoorName()
    {
        var key = new Key("key1", "brass key");
        var room = new Location("hall");
        var door = new Door("gate", "iron gate").RequiresKey(key);
        room.AddExit(Direction.North, new Location("yard"), door);
        var state = new GameState(room);
        state.Inventory.Add(key);

        var result = state.Execute(new UnlockCommand("oak door"));
        Assert.False(result.Success);
        Assert.Equal(GameError.NoDoorHere, result.Error);
    }

    // ── ItemAction reactions fire on container open/close ────────────────────

    [Fact]
    public void OpenCommand_ContainerReactionFires()
    {
        var room = new Location("hall");
        var chest = new ContainerItem<IItem>("chest", "ancient chest");
        chest.SetReaction(ItemAction.Open, "A cold breeze escapes.");
        room.AddItem(chest);
        var state = new GameState(room);

        var result = state.Execute(new OpenCommand("ancient chest"));
        Assert.True(result.Success);
        Assert.Single(result.ReactionsList);
        Assert.Equal("A cold breeze escapes.", result.ReactionsList[0]);
    }

    [Fact]
    public void CloseCommand_ContainerReactionFires()
    {
        var room = new Location("hall");
        var chest = new ContainerItem<IItem>("chest", "ancient chest");
        chest.SetReaction(ItemAction.Close, "The lid clicks shut.");
        room.AddItem(chest);
        var state = new GameState(room);

        var result = state.Execute(new CloseCommand("ancient chest"));
        Assert.True(result.Success);
        Assert.Single(result.ReactionsList);
        Assert.Equal("The lid clicks shut.", result.ReactionsList[0]);
    }
}

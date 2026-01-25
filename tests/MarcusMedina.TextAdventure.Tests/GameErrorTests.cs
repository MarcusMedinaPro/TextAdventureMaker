using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class GameErrorTests
{
    [Fact]
    public void GoCommand_NoExit_ReturnsNoExitError()
    {
        var state = new GameState(new Location("start"));

        var result = state.Execute(new GoCommand(Direction.North));

        Assert.False(result.Success);
        Assert.Equal(GameError.NoExitInDirection, result.Error);
    }

    [Fact]
    public void GoCommand_LockedDoor_ReturnsDoorLockedError()
    {
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "gate", DoorState.Locked);
        start.AddExit(Direction.North, next, door);
        var state = new GameState(start);

        var result = state.Execute(new GoCommand(Direction.North));

        Assert.False(result.Success);
        Assert.Equal(GameError.DoorIsLocked, result.Error);
    }

    [Fact]
    public void OpenCommand_NoDoor_ReturnsNoDoorError()
    {
        var start = new Location("start");
        var next = new Location("next");
        start.AddExit(Direction.East, next);
        var state = new GameState(start);

        var result = state.Execute(new OpenCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.NoDoorHere, result.Error);
    }

    [Fact]
    public void OpenCommand_LockedDoor_ReturnsDoorLockedError()
    {
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "iron door", DoorState.Locked);
        start.AddExit(Direction.East, next, door);
        var state = new GameState(start);

        var result = state.Execute(new OpenCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.DoorIsLocked, result.Error);
    }

    [Fact]
    public void OpenCommand_AlreadyOpen_ReturnsDoorAlreadyOpenError()
    {
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "iron door", DoorState.Open);
        start.AddExit(Direction.East, next, door);
        var state = new GameState(start);

        var result = state.Execute(new OpenCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.DoorAlreadyOpen, result.Error);
    }

    [Fact]
    public void UnlockCommand_NoKeyRequired_ReturnsNoKeyRequiredError()
    {
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "iron door", DoorState.Closed);
        start.AddExit(Direction.East, next, door);
        var state = new GameState(start);

        var result = state.Execute(new UnlockCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.NoKeyRequired, result.Error);
    }

    [Fact]
    public void UnlockCommand_WrongKey_ReturnsWrongKeyError()
    {
        var correct = new Key("key1", "key");
        var wrong = new Key("key2", "wrong");
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "iron door").RequiresKey(correct);
        start.AddExit(Direction.East, next, door);
        var state = new GameState(start);
        state.Inventory.Add(wrong);

        var result = state.Execute(new UnlockCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.WrongKey, result.Error);
    }

    [Fact]
    public void TakeCommand_ItemNotFound_ReturnsItemNotFoundError()
    {
        var start = new Location("start");
        var state = new GameState(start);

        var result = state.Execute(new TakeCommand("coin"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
    }

    [Fact]
    public void TakeCommand_InventoryFull_ReturnsInventoryFullError()
    {
        var start = new Location("start");
        var item = new Item("coin", "Coin");
        start.AddItem(item);
        var inventory = new Inventory(InventoryLimitType.ByCount, maxCount: 0);
        var state = new GameState(start, inventory: inventory);

        var result = state.Execute(new TakeCommand("coin"));

        Assert.False(result.Success);
        Assert.Equal(GameError.InventoryFull, result.Error);
    }

    [Fact]
    public void TakeAllCommand_NoItems_ReturnsItemNotFoundError()
    {
        var start = new Location("start");
        var state = new GameState(start);

        var result = state.Execute(new TakeAllCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
    }

    [Fact]
    public void TakeAllCommand_InventoryFull_ReturnsInventoryFullError()
    {
        var start = new Location("start");
        start.AddItem(new Item("coin", "Coin"));
        var inventory = new Inventory(InventoryLimitType.ByCount, maxCount: 0);
        var state = new GameState(start, inventory: inventory);

        var result = state.Execute(new TakeAllCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.InventoryFull, result.Error);
    }

    [Fact]
    public void DropCommand_ItemNotInInventory_ReturnsItemNotInInventoryError()
    {
        var start = new Location("start");
        var state = new GameState(start);

        var result = state.Execute(new DropCommand("coin"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotInInventory, result.Error);
    }

    [Fact]
    public void DropAllCommand_EmptyInventory_ReturnsItemNotInInventoryError()
    {
        var start = new Location("start");
        var state = new GameState(start);

        var result = state.Execute(new DropAllCommand());

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotInInventory, result.Error);
    }

    [Fact]
    public void UseCommand_ItemNotFound_ReturnsItemNotFoundError()
    {
        var start = new Location("start");
        var state = new GameState(start);

        var result = state.Execute(new UseCommand("wand"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
    }
}

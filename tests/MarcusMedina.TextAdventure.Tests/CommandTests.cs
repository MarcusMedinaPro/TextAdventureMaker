using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class CommandTests
{
    [Fact]
    public void GoCommand_MovesWhenExitExists()
    {
        var start = new Location("start");
        var next = new Location("next");
        start.AddExit(Direction.North, next);
        var state = new GameState(start);

        var result = state.Execute(new GoCommand(Direction.North));

        Assert.True(result.Success);
        Assert.Equal(next, state.CurrentLocation);
    }

    [Fact]
    public void CommandContext_NullState_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new CommandContext(null!));
    }

    [Fact]
    public void GoCommand_ReturnsErrorWhenNoExit()
    {
        var state = new GameState(new Location("start"));

        var result = state.Execute(new GoCommand(Direction.North));

        Assert.False(result.Success);
        Assert.Equal(Language.CantGoThatWay, result.Message);
    }

    [Fact]
    public void LookCommand_ListsDescriptionAndExits()
    {
        var start = new Location("start").Description("A small room.");
        var next = new Location("next");
        start.AddExit(Direction.East, next);
        var state = new GameState(start);

        var result = state.Look();

        Assert.True(result.Success);
        Assert.Contains("A small room.", result.Message);
        Assert.Contains(Language.HealthStatus(100, 100), result.Message);
        Assert.Contains(Language.ItemsHereLabel, result.Message);
        Assert.Contains("Exits:", result.Message);
        Assert.Contains("East", result.Message);
    }

    [Fact]
    public void LookCommand_CanLookAtItemInLocation()
    {
        var start = new Location("start");
        var key = new Key("key1", "Brass Key")
            .AddAliases("key")
            .Description("A small brass key.");
        start.AddItem(key);
        var state = new GameState(start);

        var result = state.Execute(new LookCommand("key"));

        Assert.True(result.Success);
        Assert.Equal("A small brass key.", result.Message);
    }

    [Fact]
    public void LookCommand_CanLookAtItemInInventory()
    {
        var start = new Location("start");
        var coin = new Item("coin", "Coin").Description("A shiny coin.");
        var state = new GameState(start);
        state.Inventory.Add(coin);

        var result = state.Execute(new LookCommand("coin"));

        Assert.True(result.Success);
        Assert.Equal("A shiny coin.", result.Message);
    }

    [Fact]
    public void LookCommand_CanLookAtDoor()
    {
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "oak door").Description("An old oak door.");
        start.AddExit(Direction.North, next, door);
        var state = new GameState(start);

        var result = state.Execute(new LookCommand("door"));

        Assert.True(result.Success);
        Assert.Equal("An old oak door.", result.Message);
    }

    [Fact]
    public void LookCommand_CanLookAtLocation()
    {
        var start = new Location("start").Description("A quiet room.");
        var state = new GameState(start);

        var result = state.Execute(new LookCommand("room"));

        Assert.True(result.Success);
        Assert.Equal("A quiet room.", result.Message);
    }

    [Fact]
    public void OpenCommand_FailsWhenNoDoor()
    {
        var start = new Location("start");
        var next = new Location("next");
        start.AddExit(Direction.East, next);
        var state = new GameState(start);

        var result = state.Execute(new OpenCommand());

        Assert.False(result.Success);
        Assert.Equal(Language.NoDoorHere, result.Message);
    }

    [Fact]
    public void OpenCommand_FailsWhenLocked()
    {
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "iron door").RequiresKey(new Key("key1", "key"));
        start.AddExit(Direction.East, next, door);
        var state = new GameState(start);

        var result = state.Execute(new OpenCommand());

        Assert.False(result.Success);
        Assert.Equal(Language.DoorLocked("iron door"), result.Message);
    }

    [Fact]
    public void UnlockCommand_FailsWhenNoKey()
    {
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "iron door").RequiresKey(new Key("key1", "key"));
        start.AddExit(Direction.East, next, door);
        var state = new GameState(start);

        var result = state.Execute(new UnlockCommand());

        Assert.False(result.Success);
        Assert.Equal(Language.YouNeedAKeyToOpenDoor, result.Message);
    }

    [Fact]
    public void UnlockCommand_SucceedsWithCorrectKey()
    {
        var key = new Key("key1", "key");
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "iron door").RequiresKey(key);
        start.AddExit(Direction.East, next, door);
        var state = new GameState(start);

        state.Inventory.Add(key);
        var result = state.Execute(new UnlockCommand());

        Assert.True(result.Success);
        Assert.Equal(Language.DoorUnlocked("iron door"), result.Message);
    }

    [Fact]
    public void StatsCommand_ReturnsHealthStatus()
    {
        var stats = new Stats(20, currentHealth: 7);
        var state = new GameState(new Location("start"), stats);

        var result = state.StatsView();

        Assert.True(result.Success);
        Assert.Equal(Language.HealthStatus(7, 20), result.Message);
    }

    [Fact]
    public void GoToCommand_GoesThroughDoor()
    {
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "iron door");
        door.Open();
        start.AddExit(Direction.North, next, door);
        var state = new GameState(start);

        var result = state.Execute(new GoToCommand("door"));

        Assert.True(result.Success);
        Assert.Equal(next, state.CurrentLocation);
    }
}

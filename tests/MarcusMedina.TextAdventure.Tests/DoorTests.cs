using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class DoorTests
{
    [Fact]
    public void Door_StartsInClosedState()
    {
        var door = new Door("door1", "wooden door");
        Assert.Equal(DoorState.Closed, door.State);
        Assert.False(door.IsPassable);
    }

    [Fact]
    public void Door_CanBeOpened()
    {
        var door = new Door("door1", "wooden door");
        Assert.True(door.Open());
        Assert.Equal(DoorState.Open, door.State);
        Assert.True(door.IsPassable);
    }

    [Fact]
    public void LockedDoor_CannotBeOpened()
    {
        var key = new Key("key1", "rusty key");
        var door = new Door("door1", "iron door").RequiresKey(key);

        Assert.False(door.Open());
        Assert.Equal(DoorState.Locked, door.State);
    }

    [Fact]
    public void LockedDoor_CanBeUnlockedWithCorrectKey()
    {
        var key = new Key("key1", "rusty key");
        var door = new Door("door1", "iron door").RequiresKey(key);

        Assert.True(door.Unlock(key));
        Assert.Equal(DoorState.Closed, door.State);
        Assert.True(door.Open());
        Assert.True(door.IsPassable);
    }

    [Fact]
    public void LockedDoor_CannotBeUnlockedWithWrongKey()
    {
        var correctKey = new Key("key1", "rusty key");
        var wrongKey = new Key("key2", "golden key");
        var door = new Door("door1", "iron door").RequiresKey(correctKey);

        Assert.False(door.Unlock(wrongKey));
        Assert.Equal(DoorState.Locked, door.State);
    }

    [Fact]
    public void Player_CannotPassThroughClosedDoor()
    {
        var outside = new Location("outside");
        var inside = new Location("inside");
        var door = new Door("door1", "heavy door");

        outside.AddExit(Direction.North, inside, door);

        var state = new GameState(outside);
        Assert.False(state.Move(Direction.North));
        Assert.Equal(outside, state.CurrentLocation);
    }

    [Fact]
    public void Player_CanPassThroughOpenDoor()
    {
        var outside = new Location("outside");
        var inside = new Location("inside");
        var door = new Door("door1", "heavy door");
        door.Open();

        outside.AddExit(Direction.North, inside, door);

        var state = new GameState(outside);
        Assert.True(state.Move(Direction.North));
        Assert.Equal(inside, state.CurrentLocation);
    }

    [Fact]
    public void Door_CanBeDestroyed()
    {
        var door = new Door("door1", "wooden door", DoorState.Locked);
        door.Destroy();

        Assert.Equal(DoorState.Destroyed, door.State);
        Assert.True(door.IsPassable);
    }
}

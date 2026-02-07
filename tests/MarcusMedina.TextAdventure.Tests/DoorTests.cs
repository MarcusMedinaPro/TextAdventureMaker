// <copyright file="DoorTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class DoorTests
{
    [Fact]
    public void Door_StartsInClosedState()
    {
        Door door = new("door1", "wooden door");
        Assert.Equal(DoorState.Closed, door.State);
        Assert.False(door.IsPassable);
    }

    [Fact]
    public void Door_InvalidIdOrName_Throws()
    {
        _ = Assert.Throws<ArgumentException>(() => new Door("", "door"));
        _ = Assert.Throws<ArgumentException>(() => new Door("id", ""));
        _ = Assert.Throws<ArgumentNullException>(() => new Door(null!, "door"));
        _ = Assert.Throws<ArgumentNullException>(() => new Door("id", null!));
    }

    [Fact]
    public void Door_CanBeOpened()
    {
        Door door = new("door1", "wooden door");
        Assert.True(door.Open());
        Assert.Equal(DoorState.Open, door.State);
        Assert.True(door.IsPassable);
    }

    [Fact]
    public void LockedDoor_CannotBeOpened()
    {
        Key key = new("key1", "rusty key");
        Door door = new Door("door1", "iron door").RequiresKey(key);

        Assert.False(door.Open());
        Assert.Equal(DoorState.Locked, door.State);
    }

    [Fact]
    public void LockedDoor_CanBeUnlockedWithCorrectKey()
    {
        Key key = new("key1", "rusty key");
        Door door = new Door("door1", "iron door").RequiresKey(key);

        Assert.True(door.Unlock(key));
        Assert.Equal(DoorState.Closed, door.State);
        Assert.True(door.Open());
        Assert.True(door.IsPassable);
    }

    [Fact]
    public void LockedDoor_CannotBeUnlockedWithWrongKey()
    {
        Key correctKey = new("key1", "rusty key");
        Key wrongKey = new("key2", "golden key");
        Door door = new Door("door1", "iron door").RequiresKey(correctKey);

        Assert.False(door.Unlock(wrongKey));
        Assert.Equal(DoorState.Locked, door.State);
    }

    [Fact]
    public void Player_CannotPassThroughClosedDoor()
    {
        Location outside = new("outside");
        Location inside = new("inside");
        Door door = new("door1", "heavy door");

        _ = outside.AddExit(Direction.North, inside, door);

        GameState state = new(outside);
        Assert.False(state.Move(Direction.North));
        Assert.Equal(outside, state.CurrentLocation);
    }

    [Fact]
    public void Player_CanPassThroughOpenDoor()
    {
        Location outside = new("outside");
        Location inside = new("inside");
        Door door = new("door1", "heavy door");
        _ = door.Open();

        _ = outside.AddExit(Direction.North, inside, door);

        GameState state = new(outside);
        Assert.True(state.Move(Direction.North));
        Assert.Equal(inside, state.CurrentLocation);
    }

    [Fact]
    public void Door_CanBeDestroyed()
    {
        Door door = new("door1", "wooden door", DoorState.Locked);
        _ = door.Destroy();

        Assert.Equal(DoorState.Destroyed, door.State);
        Assert.True(door.IsPassable);
    }

    [Fact]
    public void Door_CanHaveDescription()
    {
        Door door = new Door("door1", "wooden door")
            .Description("An old oak door with iron hinges.");

        Assert.Equal("An old oak door with iron hinges.", door.GetDescription());
    }

    [Fact]
    public void Door_Reaction_CanBeSetAndRead()
    {
        Door door = new Door("door1", "wooden door")
            .SetReaction(DoorAction.Open, "It creaks open.");

        Assert.Equal("It creaks open.", door.GetReaction(DoorAction.Open));
    }

    [Fact]
    public void Door_CanUseTupleConstructor()
    {
        Door door = (id: "gate", name: "iron gate", description: "A heavy iron gate.");

        Assert.Equal("iron gate", door.Name);
        Assert.Equal("A heavy iron gate.", door.GetDescription());
    }

    [Fact]
    public void Door_Close_DoesNothingWhenLocked()
    {
        Key key = new("key1", "rusty key");
        Door door = new Door("door1", "iron door").RequiresKey(key);

        Assert.False(door.Close());
        Assert.Equal(DoorState.Locked, door.State);
    }

    [Fact]
    public void Door_Close_ClosesWhenOpen()
    {
        Door door = new("door1", "iron door");
        _ = door.Open();

        Assert.True(door.Close());
        Assert.Equal(DoorState.Closed, door.State);
        Assert.False(door.IsPassable);
    }

    [Fact]
    public void Door_Lock_FailsWhenOpen()
    {
        Key key = new("key1", "rusty key");
        Door door = new("door1", "iron door");
        _ = door.Open();

        Assert.False(door.Lock(key));
        Assert.Equal(DoorState.Open, door.State);
    }

    [Fact]
    public void Door_Lock_FailsWhenDestroyed()
    {
        Key key = new("key1", "rusty key");
        Door door = new("door1", "iron door");
        _ = door.Destroy();

        Assert.False(door.Lock(key));
        Assert.Equal(DoorState.Destroyed, door.State);
    }

    [Fact]
    public void Door_Unlock_FailsIfNotLocked()
    {
        Key key = new("key1", "rusty key");
        Door door = new("door1", "iron door");

        Assert.False(door.Unlock(key));
        Assert.Equal(DoorState.Closed, door.State);
    }

    [Fact]
    public void Door_Open_RaisesEvent()
    {
        Door door = new("door1", "wooden door");
        int raised = 0;
        door.OnOpen += _ => raised++;

        _ = door.Open();

        Assert.Equal(1, raised);
    }

    [Fact]
    public void Door_Open_DoesNotRaiseEventWhenLocked()
    {
        Key key = new("key1", "rusty key");
        Door door = new Door("door1", "iron door").RequiresKey(key);
        int raised = 0;
        door.OnOpen += _ => raised++;

        _ = door.Open();

        Assert.Equal(0, raised);
    }

    [Fact]
    public void Door_Close_RaisesEvent()
    {
        Door door = new("door1", "iron door");
        _ = door.Open();
        int raised = 0;
        door.OnClose += _ => raised++;

        _ = door.Close();

        Assert.Equal(1, raised);
    }

    [Fact]
    public void Door_Lock_RaisesEvent()
    {
        Key key = new("key1", "rusty key");
        Door door = new("door1", "iron door");
        int raised = 0;
        door.OnLock += _ => raised++;

        _ = door.Lock(key);

        Assert.Equal(1, raised);
    }

    [Fact]
    public void Door_Unlock_RaisesEvent()
    {
        Key key = new("key1", "rusty key");
        Door door = new Door("door1", "iron door").RequiresKey(key);
        int raised = 0;
        door.OnUnlock += _ => raised++;

        _ = door.Unlock(key);

        Assert.Equal(1, raised);
    }

    [Fact]
    public void Door_Destroy_RaisesEvent()
    {
        Door door = new("door1", "iron door");
        int raised = 0;
        door.OnDestroy += _ => raised++;

        _ = door.Destroy();

        Assert.Equal(1, raised);
    }
}

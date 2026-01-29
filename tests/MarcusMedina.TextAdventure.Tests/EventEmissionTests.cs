// <copyright file="EventEmissionTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class EventEmissionTests
{
    [Fact]
    public void Move_PublishesEnterAndExitEvents()
    {
        EventSystem events = new();
        Location roomA = new("room_a");
        Location roomB = new("room_b");
        _ = roomA.AddExit(Direction.North, roomB);
        GameState state = new(roomA, eventSystem: events);

        GameEvent? exitEvent = null;
        GameEvent? enterEvent = null;
        events.Subscribe(GameEventType.ExitLocation, e => exitEvent = e);
        events.Subscribe(GameEventType.EnterLocation, e => enterEvent = e);

        _ = state.Move(Direction.North);

        Assert.Equal(roomA, exitEvent?.Location);
        Assert.Equal(roomB, enterEvent?.Location);
    }

    [Fact]
    public void TakeCommand_PublishesPickupEvent()
    {
        EventSystem events = new();
        Location location = new("camp");
        Item item = new("coin", "coin");
        location.AddItem(item);
        GameState state = new(location, eventSystem: events);

        GameEvent? pickupEvent = null;
        events.Subscribe(GameEventType.PickupItem, e => pickupEvent = e);

        _ = new TakeCommand("coin").Execute(new CommandContext(state));

        Assert.Equal(item, pickupEvent?.Item);
        Assert.Equal(location, pickupEvent?.Location);
    }

    [Fact]
    public void DropCommand_PublishesDropEvent()
    {
        EventSystem events = new();
        Location location = new("camp");
        Item item = new("coin", "coin");
        GameState state = new(location, eventSystem: events);
        _ = state.Inventory.Add(item);

        GameEvent? dropEvent = null;
        events.Subscribe(GameEventType.DropItem, e => dropEvent = e);

        _ = new DropCommand("coin").Execute(new CommandContext(state));

        Assert.Equal(item, dropEvent?.Item);
        Assert.Equal(location, dropEvent?.Location);
    }

    [Fact]
    public void TalkCommand_PublishesTalkEvent()
    {
        EventSystem events = new();
        Location location = new("camp");
        INpc npc = new Npc("fox", "fox").Dialog("Hi.");
        location.AddNpc(npc);
        GameState state = new(location, eventSystem: events);

        GameEvent? talkEvent = null;
        events.Subscribe(GameEventType.TalkToNpc, e => talkEvent = e);

        _ = new TalkCommand("fox").Execute(new CommandContext(state));

        Assert.Equal(npc, talkEvent?.Npc);
        Assert.Equal(location, talkEvent?.Location);
    }

    [Fact]
    public void OpenCommand_PublishesDoorOpenEvent()
    {
        EventSystem events = new();
        Location location = new("hall");
        Location next = new("yard");
        Door door = new("gate", "gate");
        _ = location.AddExit(Direction.North, next, door);
        GameState state = new(location, eventSystem: events);

        GameEvent? openEvent = null;
        events.Subscribe(GameEventType.OpenDoor, e => openEvent = e);

        _ = new OpenCommand().Execute(new CommandContext(state));

        Assert.Equal(door, openEvent?.Door);
        Assert.Equal(location, openEvent?.Location);
    }

    [Fact]
    public void UnlockCommand_PublishesDoorUnlockEvent()
    {
        EventSystem events = new();
        Location location = new("hall");
        Location next = new("yard");
        Key key = new("gate_key", "gate key");
        Door door = new Door("gate", "gate").RequiresKey(key);
        _ = location.AddExit(Direction.North, next, door);
        GameState state = new(location, eventSystem: events);
        _ = state.Inventory.Add(key);

        GameEvent? unlockEvent = null;
        events.Subscribe(GameEventType.UnlockDoor, e => unlockEvent = e);

        _ = new UnlockCommand().Execute(new CommandContext(state));

        Assert.Equal(door, unlockEvent?.Door);
        Assert.Equal(location, unlockEvent?.Location);
    }
}

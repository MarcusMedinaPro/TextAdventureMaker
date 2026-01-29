// <copyright file="EventSystemTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public class EventSystemTests
{
    [Fact]
    public void Publish_NotifiesSubscriber()
    {
        var events = new EventSystem();
        var location = new Location("camp");
        var state = new GameState(location);
        var called = false;

        events.Subscribe(GameEventType.EnterLocation, _ => called = true);
        events.Publish(new GameEvent(GameEventType.EnterLocation, state, location));

        Assert.True(called);
    }

    [Fact]
    public void Unsubscribe_RemovesHandler()
    {
        var events = new EventSystem();
        var location = new Location("camp");
        var state = new GameState(location);
        var called = false;
        void handler(GameEvent _) => called = true;

        events.Subscribe(GameEventType.ExitLocation, handler);
        events.Unsubscribe(GameEventType.ExitLocation, handler);
        events.Publish(new GameEvent(GameEventType.ExitLocation, state, location));

        Assert.False(called);
    }

    [Fact]
    public void Publish_NotifiesAllHandlers()
    {
        var events = new EventSystem();
        var location = new Location("camp");
        var state = new GameState(location);
        var count = 0;

        events.Subscribe(GameEventType.PickupItem, _ => count++);
        events.Subscribe(GameEventType.PickupItem, _ => count++);
        events.Publish(new GameEvent(GameEventType.PickupItem, state, location));

        Assert.Equal(2, count);
    }
}

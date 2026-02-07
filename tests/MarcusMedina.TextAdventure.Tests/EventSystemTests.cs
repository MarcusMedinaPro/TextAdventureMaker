// <copyright file="EventSystemTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class EventSystemTests
{
    [Fact]
    public void Publish_NotifiesSubscriber()
    {
        EventSystem events = new();
        Location location = new("camp");
        GameState state = new(location);
        bool called = false;

        events.Subscribe(GameEventType.EnterLocation, _ => called = true);
        events.Publish(new GameEvent(GameEventType.EnterLocation, state, location));

        Assert.True(called);
    }

    [Fact]
    public void Unsubscribe_RemovesHandler()
    {
        EventSystem events = new();
        Location location = new("camp");
        GameState state = new(location);
        bool called = false;
        void handler(GameEvent _)
        {
            called = true;
        }

        events.Subscribe(GameEventType.ExitLocation, handler);
        events.Unsubscribe(GameEventType.ExitLocation, handler);
        events.Publish(new GameEvent(GameEventType.ExitLocation, state, location));

        Assert.False(called);
    }

    [Fact]
    public void Publish_NotifiesAllHandlers()
    {
        EventSystem events = new();
        Location location = new("camp");
        GameState state = new(location);
        int count = 0;

        events.Subscribe(GameEventType.PickupItem, _ => count++);
        events.Subscribe(GameEventType.PickupItem, _ => count++);
        events.Publish(new GameEvent(GameEventType.PickupItem, state, location));

        Assert.Equal(2, count);
    }
}

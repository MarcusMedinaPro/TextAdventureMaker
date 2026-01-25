using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

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
        Action<GameEvent> handler = _ => called = true;

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

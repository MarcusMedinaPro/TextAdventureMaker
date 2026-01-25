using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class EventEmissionTests
{
    [Fact]
    public void Move_PublishesEnterAndExitEvents()
    {
        var events = new EventSystem();
        var roomA = new Location("room_a");
        var roomB = new Location("room_b");
        roomA.AddExit(Direction.North, roomB);
        var state = new GameState(roomA, eventSystem: events);

        GameEvent? exitEvent = null;
        GameEvent? enterEvent = null;
        events.Subscribe(GameEventType.ExitLocation, e => exitEvent = e);
        events.Subscribe(GameEventType.EnterLocation, e => enterEvent = e);

        state.Move(Direction.North);

        Assert.Equal(roomA, exitEvent?.Location);
        Assert.Equal(roomB, enterEvent?.Location);
    }

    [Fact]
    public void TakeCommand_PublishesPickupEvent()
    {
        var events = new EventSystem();
        var location = new Location("camp");
        var item = new Item("coin", "coin");
        location.AddItem(item);
        var state = new GameState(location, eventSystem: events);

        GameEvent? pickupEvent = null;
        events.Subscribe(GameEventType.PickupItem, e => pickupEvent = e);

        new TakeCommand("coin").Execute(new CommandContext(state));

        Assert.Equal(item, pickupEvent?.Item);
        Assert.Equal(location, pickupEvent?.Location);
    }

    [Fact]
    public void DropCommand_PublishesDropEvent()
    {
        var events = new EventSystem();
        var location = new Location("camp");
        var item = new Item("coin", "coin");
        var state = new GameState(location, eventSystem: events);
        state.Inventory.Add(item);

        GameEvent? dropEvent = null;
        events.Subscribe(GameEventType.DropItem, e => dropEvent = e);

        new DropCommand("coin").Execute(new CommandContext(state));

        Assert.Equal(item, dropEvent?.Item);
        Assert.Equal(location, dropEvent?.Location);
    }

    [Fact]
    public void TalkCommand_PublishesTalkEvent()
    {
        var events = new EventSystem();
        var location = new Location("camp");
        var npc = new Npc("fox", "fox").Dialog("Hi.");
        location.AddNpc(npc);
        var state = new GameState(location, eventSystem: events);

        GameEvent? talkEvent = null;
        events.Subscribe(GameEventType.TalkToNpc, e => talkEvent = e);

        new TalkCommand("fox").Execute(new CommandContext(state));

        Assert.Equal(npc, talkEvent?.Npc);
        Assert.Equal(location, talkEvent?.Location);
    }
}

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class GameStateTests
{
    [Fact]
    public void Move_WhenNoExit_SetsErrorMessage()
    {
        var start = new Location("start");
        var state = new GameState(start);

        var moved = state.Move(Direction.North);

        Assert.False(moved);
        Assert.Equal(Language.CantGoThatWay, state.LastMoveError);
        Assert.Equal(start, state.CurrentLocation);
    }

    [Fact]
    public void GameState_NullStartLocation_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new GameState(null!));
    }

    [Fact]
    public void Move_ClosedDoor_SetsClosedErrorMessage()
    {
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "oak door", DoorState.Closed);
        start.AddExit(Direction.East, next, door);

        var state = new GameState(start);
        var moved = state.Move(Direction.East);

        Assert.False(moved);
        Assert.Equal(Language.DoorClosed("oak door"), state.LastMoveError);
        Assert.Equal(start, state.CurrentLocation);
    }

    [Fact]
    public void Move_LockedDoor_SetsLockedErrorMessage()
    {
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "iron gate", DoorState.Locked);
        start.AddExit(Direction.East, next, door);

        var state = new GameState(start);
        var moved = state.Move(Direction.East);

        Assert.False(moved);
        Assert.Equal(Language.DoorLocked("iron gate"), state.LastMoveError);
        Assert.Equal(start, state.CurrentLocation);
    }

    [Fact]
    public void Move_OpenDoor_ClearsPreviousError()
    {
        var start = new Location("start");
        var next = new Location("next");
        var door = new Door("door1", "iron gate", DoorState.Closed);
        start.AddExit(Direction.East, next, door);

        var state = new GameState(start);
        Assert.False(state.Move(Direction.East));
        Assert.NotNull(state.LastMoveError);

        door.Open();
        Assert.True(state.Move(Direction.East));
        Assert.Null(state.LastMoveError);
        Assert.Equal(next, state.CurrentLocation);
    }
}

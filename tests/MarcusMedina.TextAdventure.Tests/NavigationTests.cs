using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class NavigationTests
{
    [Fact]
    public void Player_CanMoveNorth()
    {
        var entrance = new Location("entrance");
        var forest = new Location("forest");
        entrance.AddExit(Direction.North, forest);

        var state = new GameState(entrance);
        var moved = state.Move(Direction.North);

        Assert.True(moved);
        Assert.Equal(forest, state.CurrentLocation);
    }

    [Fact]
    public void Player_CannotMoveWhereNoExit()
    {
        var entrance = new Location("entrance");
        var state = new GameState(entrance);
        var moved = state.Move(Direction.North);

        Assert.False(moved);
        Assert.Equal(entrance, state.CurrentLocation);
    }

    [Fact]
    public void Player_CanUseInOutDirections()
    {
        var outside = new Location("outside");
        var inside = new Location("inside");
        outside.AddExit(Direction.In, inside);

        var state = new GameState(outside);

        Assert.True(state.Move(Direction.In));
        Assert.Equal(inside, state.CurrentLocation);

        Assert.True(state.Move(Direction.Out));
        Assert.Equal(outside, state.CurrentLocation);
    }
}

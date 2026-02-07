// <copyright file="GameStateTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

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
        Location start = new("start");
        GameState state = new(start);

        bool moved = state.Move(Direction.North);

        Assert.False(moved);
        Assert.Equal(Language.CantGoThatWay, state.LastMoveError);
        Assert.Equal(start, state.CurrentLocation);
    }

    [Fact]
    public void GameState_NullStartLocation_Throws()
    {
        _ = Assert.Throws<ArgumentNullException>(() => new GameState(null!));
    }

    [Fact]
    public void Move_ClosedDoor_SetsClosedErrorMessage()
    {
        Location start = new("start");
        Location next = new("next");
        Door door = new("door1", "oak door", DoorState.Closed);
        _ = start.AddExit(Direction.East, next, door);

        GameState state = new(start);
        bool moved = state.Move(Direction.East);

        Assert.False(moved);
        Assert.Equal(Language.DoorClosed("oak door"), state.LastMoveError);
        Assert.Equal(start, state.CurrentLocation);
    }

    [Fact]
    public void Move_LockedDoor_SetsLockedErrorMessage()
    {
        Location start = new("start");
        Location next = new("next");
        Door door = new("door1", "iron gate", DoorState.Locked);
        _ = start.AddExit(Direction.East, next, door);

        GameState state = new(start);
        bool moved = state.Move(Direction.East);

        Assert.False(moved);
        Assert.Equal(Language.DoorLocked("iron gate"), state.LastMoveError);
        Assert.Equal(start, state.CurrentLocation);
    }

    [Fact]
    public void Move_OpenDoor_ClearsPreviousError()
    {
        Location start = new("start");
        Location next = new("next");
        Door door = new("door1", "iron gate", DoorState.Closed);
        _ = start.AddExit(Direction.East, next, door);

        GameState state = new(start);
        Assert.False(state.Move(Direction.East));
        Assert.NotNull(state.LastMoveError);

        _ = door.Open();
        Assert.True(state.Move(Direction.East));
        Assert.Null(state.LastMoveError);
        Assert.Equal(next, state.CurrentLocation);
    }
}

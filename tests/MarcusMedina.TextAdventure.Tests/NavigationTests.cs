// <copyright file="NavigationTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public class NavigationTests
{
    [Fact]
    public void Player_CanMoveNorth()
    {
        Location entrance = new("entrance");
        Location forest = new("forest");
        _ = entrance.AddExit(Direction.North, forest);

        GameState state = new(entrance);
        var moved = state.Move(Direction.North);

        Assert.True(moved);
        Assert.Equal(forest, state.CurrentLocation);
    }

    [Fact]
    public void Player_CannotMoveWhereNoExit()
    {
        Location entrance = new("entrance");
        GameState state = new(entrance);
        var moved = state.Move(Direction.North);

        Assert.False(moved);
        Assert.Equal(entrance, state.CurrentLocation);
    }

    [Fact]
    public void Player_CanUseInOutDirections()
    {
        Location outside = new("outside");
        Location inside = new("inside");
        _ = outside.AddExit(Direction.In, inside);

        GameState state = new(outside);

        Assert.True(state.Move(Direction.In));
        Assert.Equal(inside, state.CurrentLocation);

        Assert.True(state.Move(Direction.Out));
        Assert.Equal(outside, state.CurrentLocation);
    }
}
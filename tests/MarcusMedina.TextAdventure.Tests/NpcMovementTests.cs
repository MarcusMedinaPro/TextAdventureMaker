// <copyright file="NpcMovementTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public class NpcMovementTests
{
    [Fact]
    public void FollowNpcMovement_MovesToPlayerLocationWhenDifferent()
    {
        Location npcRoom = new("cave");
        Location playerRoom = new("forest");
        GameState state = new(playerRoom);
        FollowNpcMovement movement = new();

        Assert.Equal(playerRoom, movement.GetNextLocation(npcRoom, state));
        Assert.Null(movement.GetNextLocation(playerRoom, state));
    }

    [Fact]
    public void NoNpcMovement_ReturnsNull()
    {
        NoNpcMovement movement = new();
        Location current = new("room");
        GameState state = new(current);

        Assert.Null(movement.GetNextLocation(current, state));
    }

    [Fact]
    public void Npc_UsesMovementStrategy()
    {
        Location npcRoom = new("camp");
        Location playerRoom = new("village");
        GameState state = new(playerRoom);
        var npc = new Npc("traveler", "Traveler")
            .SetMovement(new FollowNpcMovement());

        Assert.Equal(playerRoom, npc.GetNextLocation(npcRoom, state));
    }

    [Fact]
    public void PatrolNpcMovement_CyclesRoute()
    {
        Location a = new("a");
        Location b = new("b");
        Location c = new("c");
        PatrolNpcMovement movement = new([a, b, c]);
        GameState state = new(a);

        Assert.Equal(b, movement.GetNextLocation(a, state));
        Assert.Equal(c, movement.GetNextLocation(b, state));
        Assert.Equal(a, movement.GetNextLocation(c, state));
    }

    [Fact]
    public void RandomNpcMovement_PicksPassableExitTarget()
    {
        Location start = new("start");
        Location a = new("a");
        Location b = new("b");
        _ = start.AddExit(Direction.North, a);
        _ = start.AddExit(Direction.South, b);

        RandomNpcMovement movement = new(new Random(5));
        GameState state = new(start);
        var next = movement.GetNextLocation(start, state);

        Assert.NotNull(next);
        Assert.True(next == a || next == b);
    }
}

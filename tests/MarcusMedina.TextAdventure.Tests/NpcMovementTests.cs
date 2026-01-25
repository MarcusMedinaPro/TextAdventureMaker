// <copyright file="NpcMovementTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class NpcMovementTests
{
    [Fact]
    public void NoNpcMovement_ReturnsNull()
    {
        var movement = new NoNpcMovement();
        var current = new Location("room");
        var state = new GameState(current);

        Assert.Null(movement.GetNextLocation(current, state));
    }

    [Fact]
    public void RandomNpcMovement_PicksPassableExitTarget()
    {
        var start = new Location("start");
        var a = new Location("a");
        var b = new Location("b");
        start.AddExit(Direction.North, a);
        start.AddExit(Direction.South, b);

        var movement = new RandomNpcMovement(new Random(5));
        var state = new GameState(start);
        var next = movement.GetNextLocation(start, state);

        Assert.NotNull(next);
        Assert.Contains(next, new[] { a, b });
    }

    [Fact]
    public void PatrolNpcMovement_CyclesRoute()
    {
        var a = new Location("a");
        var b = new Location("b");
        var c = new Location("c");
        var movement = new PatrolNpcMovement(new[] { a, b, c });
        var state = new GameState(a);

        Assert.Equal(b, movement.GetNextLocation(a, state));
        Assert.Equal(c, movement.GetNextLocation(b, state));
        Assert.Equal(a, movement.GetNextLocation(c, state));
    }

    [Fact]
    public void FollowNpcMovement_MovesToPlayerLocationWhenDifferent()
    {
        var npcRoom = new Location("cave");
        var playerRoom = new Location("forest");
        var state = new GameState(playerRoom);
        var movement = new FollowNpcMovement();

        Assert.Equal(playerRoom, movement.GetNextLocation(npcRoom, state));
        Assert.Null(movement.GetNextLocation(playerRoom, state));
    }

    [Fact]
    public void Npc_UsesMovementStrategy()
    {
        var npcRoom = new Location("camp");
        var playerRoom = new Location("village");
        var state = new GameState(playerRoom);
        var npc = new Npc("traveler", "Traveler")
            .SetMovement(new FollowNpcMovement());

        Assert.Equal(playerRoom, npc.GetNextLocation(npcRoom, state));
    }
}

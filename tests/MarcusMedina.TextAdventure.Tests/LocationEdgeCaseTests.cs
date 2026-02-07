// <copyright file="LocationEdgeCaseTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class LocationEdgeCaseTests
{
    [Fact]
    public void AddExit_WithDoor_CreatesBidirectionalExitUsingSameDoor()
    {
        Location a = new("a");
        Location b = new("b");
        Door door = new("door1", "stone door");

        _ = a.AddExit(Direction.North, b, door);

        Assert.Same(door, a.GetExit(Direction.North)?.Door);
        Assert.Same(door, b.GetExit(Direction.South)?.Door);
        Assert.Equal(a, b.GetExit(Direction.South)?.Target);
    }

    [Fact]
    public void AddExit_WithDoor_OneWay_DoesNotCreateReturnPath()
    {
        Location a = new("a");
        Location b = new("b");
        Door door = new("door1", "stone door");

        _ = a.AddExit(Direction.North, b, door, oneWay: true);

        Assert.NotNull(a.GetExit(Direction.North));
        Assert.Null(b.GetExit(Direction.South));
    }
}

// <copyright file="SpatialAwarenessTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

public class SpatialAwarenessTests
{
    [Fact]
    public void SpatialContext_WithOpenExit_HasFullVisibility()
    {
        Location hall = new("hall");
        Location study = new("study");
        hall.AddExit(Direction.North, study);

        var spatial = new SpatialContext(hall);
        var adjacent = spatial.GetAdjacentRooms().FirstOrDefault(r => r.Direction == Direction.North);

        Assert.NotNull(adjacent);
        Assert.Equal(1.0f, adjacent.Visibility);
    }

    [Fact]
    public void SpatialContext_WithClosedDoor_HasZeroVisibility()
    {
        Location hall = new("hall");
        Location study = new("study");
        var door = new Door("oak-door", "oak door");
        hall.AddExit(Direction.North, study, door);

        var spatial = new SpatialContext(hall);
        var adjacent = spatial.GetAdjacentRooms().FirstOrDefault(r => r.Direction == Direction.North);

        Assert.NotNull(adjacent);
        Assert.Equal(0.0f, adjacent.Visibility);
    }

    [Fact]
    public void SpatialContext_WithTransparentClosedDoor_HasPartialVisibility()
    {
        Location hall = new("hall");
        Location study = new("study");
        var door = new Door("glass-door", "glass door");
        door.SetProperty("transparent", "true");
        hall.AddExit(Direction.North, study, door);

        var spatial = new SpatialContext(hall);
        var adjacent = spatial.GetAdjacentRooms().FirstOrDefault(r => r.Direction == Direction.North);

        Assert.NotNull(adjacent);
        Assert.Equal(0.5f, adjacent.Visibility);
    }

    [Fact]
    public void SpatialContext_CanSee_WithHighVisibility_ReturnsTrue()
    {
        Location hall = new("hall");
        Location study = new("study");
        hall.AddExit(Direction.North, study);

        var spatial = new SpatialContext(hall);

        Assert.True(spatial.CanSee(study));
    }

    [Fact]
    public void SpatialContext_CanSee_WithLowVisibility_ReturnsFalse()
    {
        Location hall = new("hall");
        Location study = new("study");
        var door = new Door("solid-door", "solid door");
        hall.AddExit(Direction.North, study, door);

        var spatial = new SpatialContext(hall);

        Assert.False(spatial.CanSee(study));
    }

    [Fact]
    public void SpatialContext_GetAdjacentRooms_ReturnsAllConnected()
    {
        Location hall = new("hall");
        Location study = new("study");
        Location library = new("library");
        hall.AddExit(Direction.North, study);
        hall.AddExit(Direction.South, library);

        var spatial = new SpatialContext(hall);
        var adjacent = spatial.GetAdjacentRooms().ToList();

        Assert.Equal(2, adjacent.Count);
    }

    [Fact]
    public void SpatialContext_Audibility_WithClosedDoor_IsReduced()
    {
        Location hall = new("hall");
        Location study = new("study");
        var door = new Door("wooden-door", "wooden door");
        hall.AddExit(Direction.North, study, door);

        var spatial = new SpatialContext(hall);
        var adjacent = spatial.GetAdjacentRooms().First();

        Assert.True(adjacent.Audibility > 0.0f && adjacent.Audibility < 1.0f);
    }

    [Fact]
    public void SpatialContext_CanHear_WithGoodAudibility_ReturnsTrue()
    {
        Location hall = new("hall");
        Location study = new("study");
        var door = new Door("thin-door", "thin door");
        hall.AddExit(Direction.North, study, door);

        var spatial = new SpatialContext(hall);

        Assert.True(spatial.CanHear(study));
    }
}

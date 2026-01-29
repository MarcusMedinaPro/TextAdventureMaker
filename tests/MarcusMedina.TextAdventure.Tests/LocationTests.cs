// <copyright file="LocationTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class LocationTests
{
    [Fact]
    public void Location_ShouldHaveId()
    {
        Location loc = new("cave");
        Assert.Equal("cave", loc.Id);
    }

    [Fact]
    public void Location_InvalidId_Throws()
    {
        _ = Assert.Throws<ArgumentException>(() => new Location(""));
        _ = Assert.Throws<ArgumentException>(() => new Location(" "));
        _ = Assert.Throws<ArgumentNullException>(() => new Location(null!));
    }

    [Fact]
    public void Location_ShouldHaveDescription()
    {
        Location loc = new Location("cave")
            .Description("A dark cave with glowing mushrooms");

        Assert.Equal("A dark cave with glowing mushrooms", loc.GetDescription());
    }

    [Fact]
    public void AddExit_ShouldCreateBidirectionalPassage()
    {
        Location hall = new("hall");
        Location bedroom = new("bedroom");

        _ = hall.AddExit(Direction.North, bedroom);

        Assert.Equal(bedroom, hall.GetExit(Direction.North)?.Target);
        Assert.Equal(hall, bedroom.GetExit(Direction.South)?.Target);
    }

    [Fact]
    public void AddExit_OneWay_ShouldNotCreateReturnPath()
    {
        Location hall = new("hall");
        Location pit = new("pit");

        _ = hall.AddExit(Direction.Down, pit, oneWay: true);

        Assert.Equal(pit, hall.GetExit(Direction.Down)?.Target);
        Assert.Null(pit.GetExit(Direction.Up));
    }

    [Fact]
    public void Location_CanUseDescriptionConstructorAndTuple()
    {
        Location cave = new("cave", "A dark cave.");
        Location cellar = (id: "cellar", description: "A damp cellar.");

        Assert.Equal("A dark cave.", cave.GetDescription());
        Assert.Equal("A damp cellar.", cellar.GetDescription());
    }
}

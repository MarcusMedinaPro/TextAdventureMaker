// <copyright file="WorldStateTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Models;

public class WorldStateTests
{
    [Fact]
    public void WorldState_TracksFlagsAndCounters()
    {
        var state = new WorldState();

        state.SetFlag("dragon_dead", true);
        _ = state.Increment("days", 2);

        Assert.True(state.GetFlag("dragon_dead"));
        Assert.Equal(2, state.GetCounter("days"));
    }

    [Fact]
    public void WorldState_TracksRelationships()
    {
        var state = new WorldState();

        state.SetRelationship("fox", 10);

        Assert.Equal(10, state.GetRelationship("fox"));
    }

    [Fact]
    public void WorldState_TracksTimeline()
    {
        var state = new WorldState();

        state.AddTimeline("Entered cave.");

        _ = Assert.Single(state.Timeline);
    }
}

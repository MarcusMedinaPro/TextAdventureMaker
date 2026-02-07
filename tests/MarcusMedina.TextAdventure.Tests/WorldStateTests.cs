// <copyright file="WorldStateTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class WorldStateTests
{
    [Fact]
    public void WorldState_TracksFlagsAndCounters()
    {
        WorldState state = new();

        state.SetFlag("dragon_dead", true);
        _ = state.Increment("days", 2);

        Assert.True(state.GetFlag("dragon_dead"));
        Assert.Equal(2, state.GetCounter("days"));
    }

    [Fact]
    public void WorldState_TracksRelationships()
    {
        WorldState state = new();

        state.SetRelationship("fox", 10);

        Assert.Equal(10, state.GetRelationship("fox"));
    }

    [Fact]
    public void WorldState_TracksTimeline()
    {
        WorldState state = new();

        state.AddTimeline("Entered cave.");

        _ = Assert.Single(state.Timeline);
    }
}

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class WorldStateTests
{
    [Fact]
    public void WorldState_TracksFlagsAndCounters()
    {
        var state = new WorldState();

        state.SetFlag("dragon_dead", true);
        state.Increment("days", 2);

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

        Assert.Single(state.Timeline);
    }
}

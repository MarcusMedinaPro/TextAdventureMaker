namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;
using Xunit;

public class FactionSystemTests
{
    [Fact]
    public void Threshold_FiresWhenCrossed()
    {
        var state = new GameState(new Location("start"));
        var system = new FactionSystem();
        var fired = false;

        _ = system.AddFaction("wardens")
            .OnReputationThreshold(10, _ => fired = true);

        _ = system.ModifyReputation("wardens", 5, state);
        Assert.False(fired);

        _ = system.ModifyReputation("wardens", 5, state);
        Assert.True(fired);
    }

    [Fact]
    public void NegativeThreshold_FiresWhenCrossed()
    {
        var state = new GameState(new Location("start"));
        var system = new FactionSystem();
        var fired = false;

        _ = system.AddFaction("street")
            .OnReputationThreshold(-5, _ => fired = true);

        _ = system.ModifyReputation("street", -6, state);
        Assert.True(fired);
    }
}

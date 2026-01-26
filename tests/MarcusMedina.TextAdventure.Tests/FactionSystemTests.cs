using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;
using Xunit;

namespace MarcusMedina.TextAdventure.Tests;

public class FactionSystemTests
{
    [Fact]
    public void Threshold_FiresWhenCrossed()
    {
        var state = new GameState(new Location("start"));
        var system = new FactionSystem();
        var fired = false;

        system.AddFaction("wardens")
            .OnReputationThreshold(10, _ => fired = true);

        system.ModifyReputation("wardens", 5, state);
        Assert.False(fired);

        system.ModifyReputation("wardens", 5, state);
        Assert.True(fired);
    }

    [Fact]
    public void NegativeThreshold_FiresWhenCrossed()
    {
        var state = new GameState(new Location("start"));
        var system = new FactionSystem();
        var fired = false;

        system.AddFaction("street")
            .OnReputationThreshold(-5, _ => fired = true);

        system.ModifyReputation("street", -6, state);
        Assert.True(fired);
    }
}

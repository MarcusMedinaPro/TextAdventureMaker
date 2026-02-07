
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class FactionSystemTests
{
    [Fact]
    public void Threshold_FiresWhenCrossed()
    {
        GameState state = new(new Location("start"));
        FactionSystem system = new();
        bool fired = false;

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
        GameState state = new(new Location("start"));
        FactionSystem system = new();
        bool fired = false;

        _ = system.AddFaction("street")
            .OnReputationThreshold(-5, _ => fired = true);

        _ = system.ModifyReputation("street", -6, state);
        Assert.True(fired);
    }
}

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;
using Xunit;

public class RandomEventPoolTests
{
    [Fact]
    public void EventTriggers_WhenEnabledAndChanceIsOne()
    {
        var state = new GameState(new Location("start"));
        var pool = new RandomEventPool()
            .Enable()
            .SetTriggerChance(1.0);

        var triggered = false;
        _ = pool.AddEvent("spark", 1, _ => triggered = true);

        pool.Tick(state);

        Assert.True(triggered);
    }

    [Fact]
    public void Cooldown_PreventsImmediateRepeat()
    {
        var state = new GameState(new Location("start"));
        var pool = new RandomEventPool()
            .Enable()
            .SetTriggerChance(1.0);

        var hits = 0;
        _ = pool.AddEvent("gust", 1, _ => hits++);
        _ = pool.SetCooldown("gust", 2);

        pool.Tick(state);
        pool.Tick(state);

        Assert.Equal(1, hits);
    }
}

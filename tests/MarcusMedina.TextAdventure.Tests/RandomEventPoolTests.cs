namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;

public class RandomEventPoolTests
{
    [Fact]
    public void Cooldown_PreventsImmediateRepeat()
    {
        GameState state = new(new Location("start"));
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

    [Fact]
    public void EventTriggers_WhenEnabledAndChanceIsOne()
    {
        GameState state = new(new Location("start"));
        var pool = new RandomEventPool()
            .Enable()
            .SetTriggerChance(1.0);

        var triggered = false;
        _ = pool.AddEvent("spark", 1, _ => triggered = true);

        pool.Tick(state);

        Assert.True(triggered);
    }
}
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public class TimeSystemTests
{
    [Fact]
    public void MoveLimits_TriggerRemainingAndExhaustedHandlers()
    {
        var time = new TimeSystem()
            .Enable()
            .SetMaxMoves(2);

        GameState state = new(new Location("start"), timeSystem: time);
        var remainingFired = false;
        var exhaustedFired = false;

        _ = time.OnMovesRemaining(1, _ => remainingFired = true);
        _ = time.OnMovesExhausted(_ => exhaustedFired = true);

        time.Tick(state);
        time.Tick(state);

        Assert.True(remainingFired);
        Assert.True(exhaustedFired);
    }

    [Fact]
    public void PhaseHandlers_FireOnPhaseChange()
    {
        var time = new TimeSystem()
            .Enable()
            .SetTicksPerDay(4)
            .SetStartTime(TimeOfDay.Dawn);

        GameState state = new(new Location("start"), timeSystem: time);
        var fired = false;
        _ = time.OnPhase(TimeOfDay.Night, _ => fired = true);

        time.Tick(state);
        time.Tick(state);
        time.Tick(state);

        Assert.True(fired);
    }

    [Fact]
    public void Tick_DoesNothing_WhenDisabled()
    {
        TimeSystem time = new();
        GameState state = new(new Location("start"), timeSystem: time);

        time.Tick(state);

        Assert.Equal(0, time.MovesUsed);
    }
}
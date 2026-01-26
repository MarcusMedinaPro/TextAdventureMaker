using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;
using Xunit;

namespace MarcusMedina.TextAdventure.Tests;

public class TimeSystemTests
{
    [Fact]
    public void Tick_DoesNothing_WhenDisabled()
    {
        var time = new TimeSystem();
        var state = new GameState(new Location("start"), timeSystem: time);

        time.Tick(state);

        Assert.Equal(0, time.MovesUsed);
    }

    [Fact]
    public void PhaseHandlers_FireOnPhaseChange()
    {
        var time = new TimeSystem()
            .Enable()
            .SetTicksPerDay(4)
            .SetStartTime(TimeOfDay.Dawn);

        var state = new GameState(new Location("start"), timeSystem: time);
        var fired = false;
        time.OnPhase(TimeOfDay.Night, _ => fired = true);

        time.Tick(state);
        time.Tick(state);
        time.Tick(state);

        Assert.True(fired);
    }

    [Fact]
    public void MoveLimits_TriggerRemainingAndExhaustedHandlers()
    {
        var time = new TimeSystem()
            .Enable()
            .SetMaxMoves(2);

        var state = new GameState(new Location("start"), timeSystem: time);
        var remainingFired = false;
        var exhaustedFired = false;

        time.OnMovesRemaining(1, _ => remainingFired = true);
        time.OnMovesExhausted(_ => exhaustedFired = true);

        time.Tick(state);
        time.Tick(state);

        Assert.True(remainingFired);
        Assert.True(exhaustedFired);
    }
}

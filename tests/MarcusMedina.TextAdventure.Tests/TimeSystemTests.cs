
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class TimeSystemTests
{
    [Fact]
    public void Tick_DoesNothing_WhenDisabled()
    {
        TimeSystem time = new();
        GameState state = new(new Location("start"), timeSystem: time);

        time.Tick(state);

        Assert.Equal(0, time.MovesUsed);
    }

    [Fact]
    public void PhaseHandlers_FireOnPhaseChange()
    {
        ITimeSystem time = new TimeSystem()
            .Enable()
            .SetTicksPerDay(4)
            .SetStartTime(TimeOfDay.Dawn);

        GameState state = new(new Location("start"), timeSystem: time);
        bool fired = false;
        _ = time.OnPhase(TimeOfDay.Night, _ => fired = true);

        time.Tick(state);
        time.Tick(state);
        time.Tick(state);

        Assert.True(fired);
    }

    [Fact]
    public void MoveLimits_TriggerRemainingAndExhaustedHandlers()
    {
        ITimeSystem time = new TimeSystem()
            .Enable()
            .SetMaxMoves(2);

        GameState state = new(new Location("start"), timeSystem: time);
        bool remainingFired = false;
        bool exhaustedFired = false;

        _ = time.OnMovesRemaining(1, _ => remainingFired = true);
        _ = time.OnMovesExhausted(_ => exhaustedFired = true);

        time.Tick(state);
        time.Tick(state);

        Assert.True(remainingFired);
        Assert.True(exhaustedFired);
    }
}


using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class TimedChallengeTests
{
    [Fact]
    public void Challenge_FailsWhenMovesRunOut()
    {
        ITimeSystem time = new TimeSystem().Enable();
        GameState state = new(new Location("start"), timeSystem: time);
        TimedChallenge challenge = (TimedChallenge)time.CreateTimedChallenge("bomb");
        bool failed = false;

        _ = challenge.MaxMovesLimit(2)
            .OnFailure(_ => failed = true);

        challenge.Start(state);
        challenge.Tick(state);
        challenge.Tick(state);

        Assert.True(failed);
        Assert.False(challenge.IsActive);
    }
}

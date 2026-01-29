namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;
using Xunit;

public class TimedChallengeTests
{
    [Fact]
    public void Challenge_FailsWhenMovesRunOut()
    {
        var time = new TimeSystem().Enable();
        var state = new GameState(new Location("start"), timeSystem: time);
        var challenge = (TimedChallenge)time.CreateTimedChallenge("bomb");
        var failed = false;

        _ = challenge.MaxMovesLimit(2)
            .OnFailure(_ => failed = true);

        challenge.Start(state);
        challenge.Tick(state);
        challenge.Tick(state);

        Assert.True(failed);
        Assert.False(challenge.IsActive);
    }
}

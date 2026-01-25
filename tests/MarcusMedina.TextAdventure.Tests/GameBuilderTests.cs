using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class GameBuilderTests
{
    [Fact]
    public void Build_UsesStartLocationAndRegistersLocations()
    {
        var start = new Location("start");
        var other = new Location("other");
        var parser = new StubParser(new LookCommand());

        var game = GameBuilder.Create()
            .AddLocation(start, isStart: true)
            .AddLocation(other)
            .UseParser(parser)
            .Build();

        Assert.Equal(start, game.State.CurrentLocation);
        Assert.Contains(other, game.State.Locations);
    }

    [Fact]
    public void TickNpcs_MovesNpc()
    {
        var start = new Location("start");
        var other = new Location("other");
        var npc = new Npc("ghost", "ghost")
            .SetMovement(new FixedMovement(other));

        start.AddNpc(npc);

        var state = new GameState(start, worldLocations: new[] { start, other });
        var game = new Game(state, new StubParser(new LookCommand()));

        game.TickNpcs();

        Assert.DoesNotContain(npc, start.Npcs);
        Assert.Contains(npc, other.Npcs);
    }

    private sealed class StubParser : ICommandParser
    {
        private readonly ICommand _command;

        public StubParser(ICommand command)
        {
            _command = command;
        }

        public ICommand Parse(string input) => _command;
    }

    private sealed class FixedMovement : INpcMovement
    {
        private readonly ILocation _target;

        public FixedMovement(ILocation target)
        {
            _target = target;
        }

        public ILocation? GetNextLocation(ILocation currentLocation, IGameState state)
        {
            return _target;
        }
    }
}

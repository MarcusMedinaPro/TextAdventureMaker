// <copyright file="GameBuilderTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

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
        Location start = new("start");
        Location other = new("other");
        StubParser parser = new(new LookCommand());

        Game game = GameBuilder.Create()
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
        Location start = new("start");
        Location other = new("other");
        INpc npc = new Npc("ghost", "ghost")
            .SetMovement(new FixedMovement(other));

        start.AddNpc(npc);

        GameState state = new(start, worldLocations: new[] { start, other });
        Game game = new(state, new StubParser(new LookCommand()));

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

        public ICommand Parse(string input)
        {
            return _command;
        }
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

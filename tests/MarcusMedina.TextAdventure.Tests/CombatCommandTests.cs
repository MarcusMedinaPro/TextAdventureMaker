// <copyright file="CombatCommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

public class CombatCommandTests
{
    private sealed class StubCombatSystem : ICombatSystem
    {
        public CommandResult Result { get; set; } = CommandResult.Ok("ok");
        public INpc? LastTarget { get; private set; }

        public CommandResult Attack(IGameState state, INpc target)
        {
            LastTarget = target;
            return Result;
        }

        public CommandResult Flee(IGameState state, INpc? target = null)
        {
            LastTarget = target;
            return Result;
        }
    }

    [Fact]
    public void AttackCommand_RequiresTarget()
    {
        var location = new Location("arena");
        var state = new GameState(location);

        var result = new AttackCommand(null).Execute(new CommandContext(state));

        Assert.False(result.Success);
        Assert.Equal(Language.NoTargetToAttack, result.Message);
        Assert.Equal(GameError.MissingArgument, result.Error);
    }

    [Fact]
    public void AttackCommand_UsesCombatSystemAndPublishesEvent()
    {
        var events = new EventSystem();
        var combat = new StubCombatSystem { Result = CommandResult.Ok("strike") };
        var location = new Location("arena");
        var npc = new Npc("rat", "rat");
        location.AddNpc(npc);
        var state = new GameState(location, eventSystem: events, combatSystem: combat);

        GameEvent? combatEvent = null;
        events.Subscribe(GameEventType.CombatStart, e => combatEvent = e);

        var result = new AttackCommand("rat").Execute(new CommandContext(state));

        Assert.True(result.Success);
        Assert.Equal("strike", result.Message);
        Assert.Equal(npc, combat.LastTarget);
        Assert.Equal(npc, combatEvent?.Npc);
    }

    [Fact]
    public void FleeCommand_FailsWhenNoNpcPresent()
    {
        var location = new Location("arena");
        var state = new GameState(location);

        var result = new FleeCommand().Execute(new CommandContext(state));

        Assert.False(result.Success);
        Assert.Equal(Language.NoOneToFlee, result.Message);
        Assert.Equal(GameError.TargetNotFound, result.Error);
    }

    [Fact]
    public void FleeCommand_UsesCombatSystemWhenNpcPresent()
    {
        var combat = new StubCombatSystem { Result = CommandResult.Ok("escape") };
        var location = new Location("arena");
        var npc = new Npc("rat", "rat");
        location.AddNpc(npc);
        var state = new GameState(location, combatSystem: combat);

        var result = new FleeCommand().Execute(new CommandContext(state));

        Assert.True(result.Success);
        Assert.Equal("escape", result.Message);
        Assert.Equal(npc, combat.LastTarget);
    }
}

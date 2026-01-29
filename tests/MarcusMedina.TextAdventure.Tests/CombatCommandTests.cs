// <copyright file="CombatCommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

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
        Location location = new("arena");
        GameState state = new(location);

        CommandResult result = new AttackCommand(null).Execute(new CommandContext(state));

        Assert.False(result.Success);
        Assert.Equal(Language.NoTargetToAttack, result.Message);
        Assert.Equal(GameError.MissingArgument, result.Error);
    }

    [Fact]
    public void AttackCommand_UsesCombatSystemAndPublishesEvent()
    {
        EventSystem events = new();
        StubCombatSystem combat = new() { Result = CommandResult.Ok("strike") };
        Location location = new("arena");
        Npc npc = new("rat", "rat");
        location.AddNpc(npc);
        GameState state = new(location, eventSystem: events, combatSystem: combat);

        GameEvent? combatEvent = null;
        events.Subscribe(GameEventType.CombatStart, e => combatEvent = e);

        CommandResult result = new AttackCommand("rat").Execute(new CommandContext(state));

        Assert.True(result.Success);
        Assert.Equal("strike", result.Message);
        Assert.Equal(npc, combat.LastTarget);
        Assert.Equal(npc, combatEvent?.Npc);
    }

    [Fact]
    public void FleeCommand_FailsWhenNoNpcPresent()
    {
        Location location = new("arena");
        GameState state = new(location);

        CommandResult result = new FleeCommand().Execute(new CommandContext(state));

        Assert.False(result.Success);
        Assert.Equal(Language.NoOneToFlee, result.Message);
        Assert.Equal(GameError.TargetNotFound, result.Error);
    }

    [Fact]
    public void FleeCommand_UsesCombatSystemWhenNpcPresent()
    {
        StubCombatSystem combat = new() { Result = CommandResult.Ok("escape") };
        Location location = new("arena");
        Npc npc = new("rat", "rat");
        location.AddNpc(npc);
        GameState state = new(location, combatSystem: combat);

        CommandResult result = new FleeCommand().Execute(new CommandContext(state));

        Assert.True(result.Success);
        Assert.Equal("escape", result.Message);
        Assert.Equal(npc, combat.LastTarget);
    }
}

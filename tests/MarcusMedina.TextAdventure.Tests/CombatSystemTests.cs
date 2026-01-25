// <copyright file="CombatSystemTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class CombatSystemTests
{
    [Fact]
    public void TurnBasedCombat_Attack_DamagesBothSides()
    {
        var combat = new TurnBasedCombat(playerDamage: 5, npcDamage: 3);
        var location = new Location("arena");
        var state = new GameState(location);
        var npc = new Npc("bandit", "Bandit", stats: new Stats(10));

        var result = combat.Attack(state, npc);

        Assert.True(result.Success);
        Assert.Equal(5, npc.Stats.Health);
        Assert.Equal(97, state.Stats.Health);
    }

    [Fact]
    public void TurnBasedCombat_Attack_CanDefeatTarget()
    {
        var combat = new TurnBasedCombat(playerDamage: 20, npcDamage: 3);
        var location = new Location("arena");
        var state = new GameState(location);
        var npc = new Npc("bandit", "Bandit", stats: new Stats(10));

        var result = combat.Attack(state, npc);

        Assert.True(result.Success);
        Assert.False(npc.IsAlive);
        Assert.Equal(0, npc.Stats.Health);
        Assert.Equal(100, state.Stats.Health);
    }
}

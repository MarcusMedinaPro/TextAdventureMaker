// <copyright file="QuestConditionTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public class QuestConditionTests
{
    [Fact]
    public void HasItemCondition_IsMetWhenItemInInventory()
    {
        var location = new Location("camp");
        var state = new GameState(location);
        _ = state.Inventory.Add(new Item("sword", "sword"));
        var condition = new HasItemCondition("sword");

        var evaluator = new QuestConditionEvaluator(state);

        Assert.True(condition.Accept(evaluator));
    }

    [Fact]
    public void NpcStateCondition_IsMetWhenNpcMatchesState()
    {
        var npc = new Npc("dragon", "Dragon").SetState(NpcState.Dead);
        var condition = new NpcStateCondition(npc, NpcState.Dead);

        var evaluator = new QuestConditionEvaluator(new GameState(new Location("camp")));

        Assert.True(condition.Accept(evaluator));
    }

    [Fact]
    public void Quest_CheckProgress_CompletesWhenConditionsMet()
    {
        var location = new Location("camp");
        var state = new GameState(location);
        _ = state.Inventory.Add(new Item("sword", "sword"));
        var npc = new Npc("dragon", "Dragon").SetState(NpcState.Dead);
        state.WorldState.SetFlag("dragon_defeated", true);
        _ = state.WorldState.Increment("villagers_saved", 2);
        state.WorldState.SetRelationship("fox", 5);

        var quest = new Quest("dragon_hunt", "Dragon Hunt", "Find the sword and slay the dragon.")
            .AddCondition(new HasItemCondition("sword"))
            .AddCondition(new NpcStateCondition(npc, NpcState.Dead))
            .AddCondition(new WorldFlagCondition("dragon_defeated"))
            .AddCondition(new WorldCounterCondition("villagers_saved", 2))
            .AddCondition(new RelationshipCondition("fox", 5))
            .Start();

        var completed = quest.CheckProgress(state);

        Assert.True(completed);
        Assert.Equal(QuestState.Completed, quest.State);
    }
}

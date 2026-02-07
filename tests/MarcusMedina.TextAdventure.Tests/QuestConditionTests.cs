// <copyright file="QuestConditionTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class QuestConditionTests
{
    [Fact]
    public void HasItemCondition_IsMetWhenItemInInventory()
    {
        Location location = new("camp");
        GameState state = new(location);
        _ = state.Inventory.Add(new Item("sword", "sword"));
        HasItemCondition condition = new("sword");

        QuestConditionEvaluator evaluator = new(state);

        Assert.True(condition.Accept(evaluator));
    }

    [Fact]
    public void NpcStateCondition_IsMetWhenNpcMatchesState()
    {
        INpc npc = new Npc("dragon", "Dragon").SetState(NpcState.Dead);
        NpcStateCondition condition = new(npc, NpcState.Dead);

        QuestConditionEvaluator evaluator = new(new GameState(new Location("camp")));

        Assert.True(condition.Accept(evaluator));
    }

    [Fact]
    public void Quest_CheckProgress_CompletesWhenConditionsMet()
    {
        Location location = new("camp");
        GameState state = new(location);
        _ = state.Inventory.Add(new Item("sword", "sword"));
        INpc npc = new Npc("dragon", "Dragon").SetState(NpcState.Dead);
        state.WorldState.SetFlag("dragon_defeated", true);
        _ = state.WorldState.Increment("villagers_saved", 2);
        state.WorldState.SetRelationship("fox", 5);

        IQuest quest = new Quest("dragon_hunt", "Dragon Hunt", "Find the sword and slay the dragon.")
            .AddCondition(new HasItemCondition("sword"))
            .AddCondition(new NpcStateCondition(npc, NpcState.Dead))
            .AddCondition(new WorldFlagCondition("dragon_defeated"))
            .AddCondition(new WorldCounterCondition("villagers_saved", 2))
            .AddCondition(new RelationshipCondition("fox", 5))
            .Start();

        bool completed = quest.CheckProgress(state);

        Assert.True(completed);
        Assert.Equal(QuestState.Completed, quest.State);
    }
}

// <copyright file="QuestLogTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class QuestLogTests
{
    [Fact]
    public void Add_StoresQuestOnce()
    {
        var log = new QuestLog();
        var quest = new Quest("find_note", "Find the Note", "Locate the sticky note.");

        log.Add(quest);
        log.Add(quest);

        Assert.Single(log.Quests);
    }

    [Fact]
    public void CheckAll_CompletesMatchingQuest()
    {
        var room = new Location("office", "A quiet office.");
        var state = new GameState(room, worldLocations: new[] { room });
        var quest = new Quest("login", "Log In", "Remember the password.")
            .AddCondition(new WorldFlagCondition("knows_password"))
            .Start();

        state.Quests.Add(quest);
        state.WorldState.SetFlag("knows_password", true);

        var completed = state.Quests.CheckAll(state);

        Assert.True(completed);
        Assert.Equal(QuestState.Completed, quest.State);
    }
}

// <copyright file="QuestLogTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class QuestLogTests
{
    [Fact]
    public void Add_StoresQuestOnce()
    {
        QuestLog log = new();
        Quest quest = new("find_note", "Find the Note", "Locate the sticky note.");

        _ = log.Add(quest);
        _ = log.Add(quest);

        _ = Assert.Single(log.Quests);
    }

    [Fact]
    public void CheckAll_CompletesMatchingQuest()
    {
        Location room = new("office", "A quiet office.");
        GameState state = new(room, worldLocations: new[] { room });
        IQuest quest = new Quest("login", "Log In", "Remember the password.")
            .AddCondition(new WorldFlagCondition("knows_password"))
            .Start();

        _ = state.Quests.Add(quest);
        state.WorldState.SetFlag("knows_password", true);

        bool completed = state.Quests.CheckAll(state);

        Assert.True(completed);
        Assert.Equal(QuestState.Completed, quest.State);
    }
}

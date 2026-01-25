// <copyright file="QuestTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class QuestTests
{
    [Fact]
    public void Quest_Start_TransitionsToActive()
    {
        var quest = new Quest("find_sword", "Find the Sword", "Retrieve the lost sword.");

        quest.Start();

        Assert.Equal(QuestState.Active, quest.State);
    }

    [Fact]
    public void Quest_Complete_TransitionsToCompleted()
    {
        var quest = new Quest("find_sword", "Find the Sword", "Retrieve the lost sword.");

        quest.Complete();

        Assert.Equal(QuestState.Completed, quest.State);
    }

    [Fact]
    public void Quest_Fail_TransitionsToFailed()
    {
        var quest = new Quest("find_sword", "Find the Sword", "Retrieve the lost sword.");

        quest.Fail();

        Assert.Equal(QuestState.Failed, quest.State);
    }
}

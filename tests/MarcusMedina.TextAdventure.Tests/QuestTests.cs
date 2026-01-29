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
        Quest quest = new("find_sword", "Find the Sword", "Retrieve the lost sword.");

        _ = quest.Start();

        Assert.Equal(QuestState.Active, quest.State);
    }

    [Fact]
    public void Quest_Complete_TransitionsToCompleted()
    {
        Quest quest = new("find_sword", "Find the Sword", "Retrieve the lost sword.");

        _ = quest.Complete();

        Assert.Equal(QuestState.Completed, quest.State);
    }

    [Fact]
    public void Quest_Fail_TransitionsToFailed()
    {
        Quest quest = new("find_sword", "Find the Sword", "Retrieve the lost sword.");

        _ = quest.Fail();

        Assert.Equal(QuestState.Failed, quest.State);
    }
}

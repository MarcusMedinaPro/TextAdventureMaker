// <copyright file="IQuest.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IQuest
{
    string Id { get; }
    string Title { get; }
    string Description { get; }
    QuestState State { get; }
    IReadOnlyList<IQuestCondition> Conditions { get; }

    IQuest Start();
    IQuest Complete();
    IQuest Fail();
    IQuest AddCondition(IQuestCondition condition);
    bool CheckProgress(IGameState state);
}

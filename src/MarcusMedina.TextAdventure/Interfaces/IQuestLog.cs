// <copyright file="IQuestLog.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

using MarcusMedina.TextAdventure.Enums;

/// <summary>Tracks quests and their current states.</summary>
public interface IQuestLog
{
    IReadOnlyList<IQuest> Quests { get; }

    IQuest Add(IQuest quest);

    void AddRange(IEnumerable<IQuest> quests);

    bool CheckAll(IGameState state);

    IQuest? Find(string id);

    IReadOnlyList<IQuest> GetByState(QuestState state);
}
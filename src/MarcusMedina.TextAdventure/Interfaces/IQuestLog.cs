// <copyright file="IQuestLog.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

using System.Collections.Generic;
using MarcusMedina.TextAdventure.Enums;

/// <summary>Tracks quests and their current states.</summary>
public interface IQuestLog
{
    IReadOnlyList<IQuest> Quests { get; }

    IQuest? Find(string id);
    IQuest Add(IQuest quest);
    void AddRange(IEnumerable<IQuest> quests);
    IReadOnlyList<IQuest> GetByState(QuestState state);
    bool CheckAll(IGameState state);
}

// <copyright file="QuestLog.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

/// <summary>Simple quest log for tracking and updating quests.</summary>
public sealed class QuestLog : IQuestLog
{
    private readonly List<IQuest> _quests = [];

    public IReadOnlyList<IQuest> Quests => _quests;

    public IQuest Add(IQuest quest)
    {
        ArgumentNullException.ThrowIfNull(quest);
        if (Find(quest.Id) is { } existing)
            return existing;

        _quests.Add(quest);
        return quest;
    }

    public void AddRange(IEnumerable<IQuest> quests)
    {
        if (quests is null)
            return;

        foreach (var quest in quests)
        {
            if (quest is not null)
                _ = Add(quest);
        }
    }

    public bool CheckAll(IGameState state)
    {
        if (state is null)
            return false;

        var anyCompleted = false;
        foreach (var quest in _quests)
        {
            if (quest.CheckProgress(state))
                anyCompleted = true;
        }

        return anyCompleted;
    }

    public IQuest? Find(string id) => string.IsNullOrWhiteSpace(id) ? null : _quests.FirstOrDefault(q => q.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<IQuest> GetByState(QuestState state) => [.._quests.Where(q => q.State == state)];
}

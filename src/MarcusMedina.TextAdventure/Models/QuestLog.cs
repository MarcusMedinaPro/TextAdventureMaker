// <copyright file="QuestLog.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;
/// <summary>Simple quest log for tracking and updating quests.</summary>
public sealed class QuestLog : IQuestLog
{
    private readonly List<IQuest> _quests = [];

    public IReadOnlyList<IQuest> Quests => _quests;

    public IQuest? Find(string id)
    {
        return string.IsNullOrWhiteSpace(id) ? null : _quests.FirstOrDefault(q => q.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }

    public IQuest Add(IQuest quest)
    {
        ArgumentNullException.ThrowIfNull(quest);
        IQuest? existing = Find(quest.Id);
        if (existing != null)
        {
            return existing;
        }

        _quests.Add(quest);
        return quest;
    }

    public void AddRange(IEnumerable<IQuest> quests)
    {
        if (quests == null)
        {
            return;
        }

        foreach (IQuest quest in quests)
        {
            if (quest != null)
            {
                _ = Add(quest);
            }
        }
    }

    public IReadOnlyList<IQuest> GetByState(QuestState state)
    {
        return _quests.Where(q => q.State == state).ToList();
    }

    public bool CheckAll(IGameState state)
    {
        if (state == null)
        {
            return false;
        }

        bool anyCompleted = false;
        foreach (IQuest quest in _quests)
        {
            if (quest.CheckProgress(state))
            {
                anyCompleted = true;
            }
        }

        return anyCompleted;
    }
}

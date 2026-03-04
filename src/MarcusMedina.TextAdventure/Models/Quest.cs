// <copyright file="Quest.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

public class Quest : IQuest
{
    private readonly List<IQuestCondition> _conditions = [];

    public Quest(string id, string title, string description, QuestState state = QuestState.Inactive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Id = id;
        Title = title;
        Description = description;
        State = state;
    }

    public IReadOnlyList<IQuestCondition> Conditions => _conditions;
    public string Description { get; }
    public string Id { get; }
    public QuestState State { get; private set; }
    public string Title { get; }

    public IQuest AddCondition(IQuestCondition condition)
    {
        ArgumentNullException.ThrowIfNull(condition);
        _conditions.Add(condition);
        return this;
    }

    public bool CheckProgress(IGameState state)
    {
        if (State is not QuestState.Active || _conditions.Count == 0)
            return false;

        QuestConditionEvaluator evaluator = new(state);
        if (!_conditions.All(condition => condition.Accept(evaluator)))
            return false;

        State = QuestState.Completed;
        return true;
    }

    public IQuest Complete()
    {
        State = QuestState.Completed;
        return this;
    }

    public IQuest Fail()
    {
        State = QuestState.Failed;
        return this;
    }

    public IQuest Start()
    {
        if (State is QuestState.Inactive)
            State = QuestState.Active;

        return this;
    }
}
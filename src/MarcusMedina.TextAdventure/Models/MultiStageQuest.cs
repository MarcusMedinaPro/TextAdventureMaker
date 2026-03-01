// <copyright file="MultiStageQuest.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class MultiStageQuest
{
    private readonly List<QuestStage> _stages = [];
    private int _currentIndex;

    public string Id { get; }
    public string Title { get; }
    public string Description { get; }
    public IReadOnlyList<QuestStage> Stages => _stages;
    public QuestStage? CurrentStage => _currentIndex < _stages.Count ? _stages[_currentIndex] : null;

    public MultiStageQuest(string id, string title, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Id = id;
        Title = title;
        Description = description;
    }

    public QuestStage AddStage(string id)
    {
        QuestStage stage = new(id);
        _stages.Add(stage);
        return stage;
    }

    public bool CheckProgress(IGameState state)
    {
        QuestStage? stage = CurrentStage;
        if (stage == null)
        {
            return false;
        }

        if (!stage.CheckCompleted(state))
        {
            return false;
        }

        _currentIndex++;
        return true;
    }
}

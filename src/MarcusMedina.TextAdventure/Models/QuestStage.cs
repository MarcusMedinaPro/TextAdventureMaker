// <copyright file="QuestStage.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class QuestStage(string id) : IQuestStage
{
    private readonly List<QuestObjective> _objectives = [];
    private readonly HashSet<string> _alternativePaths = new(StringComparer.OrdinalIgnoreCase);
    private Action<IGameState>? _onFailure;
    private Action<IGameState>? _onComplete;

    public string Id { get; } = id is not null && id.Trim().Length > 0 ? id : throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));
    public bool IsCompleted { get; private set; }
    public IReadOnlyList<IQuestObjective> Objectives => _objectives;

    public QuestStage RequireObjective(string id)
    {
        _objectives.Add(new QuestObjective(id, isOptional: false));
        return this;
    }

    public QuestStage OptionalObjective(string id)
    {
        _objectives.Add(new QuestObjective(id, isOptional: true));
        return this;
    }

    public QuestStage AlternativePath(string id)
    {
        _alternativePaths.Add(id);
        return this;
    }

    public QuestStage OnFailure(Action<IGameState> action)
    {
        _onFailure = action;
        return this;
    }

    public QuestStage OnComplete(Action<IGameState> action)
    {
        _onComplete = action;
        return this;
    }

    public void CompleteObjective(string id) =>
        _objectives.FirstOrDefault(o => o.Id.Equals(id, StringComparison.OrdinalIgnoreCase))?.Complete();

    public void CompleteAlternative(string id)
    {
        if (_alternativePaths.Contains(id))
            IsCompleted = true;
    }

    public bool CheckCompleted(IGameState state)
    {
        if (IsCompleted)
            return true;

        if (!_objectives.Where(o => !o.IsOptional).All(o => o.IsCompleted))
            return false;

        IsCompleted = true;
        _onComplete?.Invoke(state);
        return true;
    }

    public void Fail(IGameState state) => _onFailure?.Invoke(state);
}

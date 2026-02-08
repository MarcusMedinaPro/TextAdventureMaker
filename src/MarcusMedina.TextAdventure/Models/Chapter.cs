// <copyright file="Chapter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class Chapter(string id, string title) : IChapter
{
    private readonly List<ChapterObjective> _objectives = [];
    private readonly List<string> _convergesFrom = [];
    private readonly List<ChapterEnding> _endings = [];
    private Func<IChapterSystem, bool>? _unlockCondition;
    private Action<ChapterContext>? _onComplete;
    private string? _nextChapterId;

    public string Id { get; } = id ?? "";
    public string Title { get; } = title ?? "";
    public ChapterState State { get; private set; } = ChapterState.NotStarted;
    public IEnumerable<IChapterObjective> Objectives => _objectives;
    public IChapter? NextChapter { get; internal set; }
    public IReadOnlyCollection<ChapterEnding> Endings => _endings;

    public Chapter SetNext(string chapterId)
    {
        _nextChapterId = chapterId;
        return this;
    }

    public string? NextChapterId => _nextChapterId;

    public Chapter UnlockedWhen(Func<IChapterSystem, bool> predicate)
    {
        _unlockCondition = predicate;
        return this;
    }

    public Chapter OnComplete(Action<ChapterContext> handler)
    {
        _onComplete = handler;
        return this;
    }

    public Chapter AddObjective(ChapterObjective objective)
    {
        _objectives.Add(objective);
        return this;
    }

    public Chapter ConvergesFrom(params string[] chapterIds)
    {
        if (chapterIds != null)
        {
            _convergesFrom.AddRange(chapterIds.Where(id => !string.IsNullOrWhiteSpace(id)));
        }

        return this;
    }

    public Chapter AddEnding(ChapterEnding ending)
    {
        _endings.Add(ending);
        return this;
    }

    public bool CanActivate(IChapterSystem system)
    {
        return _unlockCondition?.Invoke(system) ?? true;
    }

    public void Activate()
    {
        State = ChapterState.Active;
    }

    public void Complete(IChapterSystem system)
    {
        State = ChapterState.Completed;
        _onComplete?.Invoke(new ChapterContext(system, this));
    }

    public void Skip()
    {
        State = ChapterState.Skipped;
    }

    public bool IsComplete(string objectiveId)
    {
        return _objectives.Any(obj => obj.Id.Equals(objectiveId, StringComparison.OrdinalIgnoreCase) && obj.IsComplete);
    }

    public ChapterObjective? FindObjective(string objectiveId)
    {
        return _objectives.FirstOrDefault(obj => obj.Id.Equals(objectiveId, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyCollection<string> ConvergeSources => _convergesFrom;
}

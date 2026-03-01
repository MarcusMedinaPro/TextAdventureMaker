// <copyright file="ChapterSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class ChapterSystem : IChapterSystem
{
    private readonly Dictionary<string, Chapter> _chapters = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _order = [];

    public IReadOnlyCollection<IChapter> Chapters => _chapters.Values;
    public IChapter? CurrentChapter { get; private set; }

    public Chapter AddChapter(Chapter chapter)
    {
        _chapters[chapter.Id] = chapter;
        if (!_order.Contains(chapter.Id))
        {
            _order.Add(chapter.Id);
        }

        return chapter;
    }

    public void LinkChaptersByOrder()
    {
        for (int i = 0; i < _order.Count - 1; i++)
        {
            Chapter current = _chapters[_order[i]];
            Chapter next = _chapters[_order[i + 1]];
            current.NextChapter = next;
        }
    }

    public void ActivateChapter(string id)
    {
        if (!_chapters.TryGetValue(id, out Chapter? chapter))
        {
            return;
        }

        if (!chapter.CanActivate(this))
        {
            return;
        }

        CurrentChapter = chapter;
        chapter.Activate();
    }

    public void AdvanceChapter()
    {
        if (CurrentChapter is not Chapter chapter)
        {
            return;
        }

        chapter.Complete(this);
        if (chapter.NextChapter is Chapter next)
        {
            ActivateChapter(next.Id);
        }
    }

    public void CompleteObjective(string objectiveId)
    {
        if (CurrentChapter is not Chapter chapter || string.IsNullOrWhiteSpace(objectiveId))
        {
            return;
        }

        ChapterObjective? objective = chapter.FindObjective(objectiveId);
        if (objective == null)
        {
            return;
        }

        objective.Complete();

        bool allRequired = chapter.Objectives
            .OfType<ChapterObjective>()
            .Where(obj => obj.IsRequired)
            .All(obj => obj.IsComplete);

        if (allRequired)
        {
            chapter.Complete(this);
        }
    }

    public bool IsComplete(string chapterId)
    {
        return _chapters.TryGetValue(chapterId, out Chapter? chapter) && chapter.State == Enums.ChapterState.Completed;
    }
}

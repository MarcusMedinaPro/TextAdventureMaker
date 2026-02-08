// <copyright file="ChapterBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class ChapterBuilder
{
    private readonly ChapterSystem _system = new();

    public ChapterDefinitionBuilder Chapter(string id, string title)
    {
        Chapter chapter = new(id, title);
        _system.AddChapter(chapter);
        return new ChapterDefinitionBuilder(this, chapter);
    }

    public ChapterSystem Build()
    {
        _system.LinkChaptersByOrder();
        return _system;
    }
}

public sealed class ChapterDefinitionBuilder(ChapterBuilder root, Chapter chapter)
{
    public ChapterDefinitionBuilder Objectives(Action<ChapterObjectivesBuilder> configure)
    {
        ChapterObjectivesBuilder builder = new(chapter);
        configure?.Invoke(builder);
        return this;
    }

    public ChapterDefinitionBuilder OnComplete(Action<ChapterContext> handler)
    {
        chapter.OnComplete(handler);
        return this;
    }

    public ChapterDefinitionBuilder UnlockedWhen(Func<IChapterSystem, bool> predicate)
    {
        chapter.UnlockedWhen(predicate);
        return this;
    }

    public ChapterDefinitionBuilder ConvergesFrom(params string[] chapterIds)
    {
        chapter.ConvergesFrom(chapterIds);
        return this;
    }

    public ChapterDefinitionBuilder MultipleEndings(Action<ChapterEndingsBuilder> configure)
    {
        ChapterEndingsBuilder builder = new(chapter);
        configure?.Invoke(builder);
        return this;
    }

    public ChapterBuilder Chapter(string id, string title)
    {
        return root.Chapter(id, title);
    }
}

public sealed class ChapterObjectivesBuilder(Chapter chapter)
{
    public ChapterObjectivesBuilder Required(string id)
    {
        chapter.AddObjective(new ChapterObjective(id, true));
        return this;
    }

    public ChapterObjectivesBuilder Optional(string id)
    {
        chapter.AddObjective(new ChapterObjective(id, false));
        return this;
    }

    public ChapterObjectivesBuilder Branch(string id, string leadsTo)
    {
        chapter.AddObjective(new ChapterObjective(id, false, leadsTo));
        return this;
    }
}

public sealed class ChapterEndingsBuilder(Chapter chapter)
{
    public ChapterEndingsBuilder Ending(string id, Func<IGameState, bool>? when = null, bool isDefault = false)
    {
        chapter.AddEnding(new ChapterEnding(id, when, isDefault));
        return this;
    }
}

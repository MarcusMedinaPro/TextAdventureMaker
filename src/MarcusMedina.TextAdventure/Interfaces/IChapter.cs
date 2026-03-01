// <copyright file="IChapter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IChapter
{
    string Id { get; }
    string Title { get; }
    ChapterState State { get; }
    IEnumerable<IChapterObjective> Objectives { get; }
    IChapter? NextChapter { get; }
}

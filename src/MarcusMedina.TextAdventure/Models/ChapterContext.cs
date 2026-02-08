// <copyright file="ChapterContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class ChapterContext(IChapterSystem system, Chapter chapter)
{
    public IChapterSystem System { get; } = system;
    public Chapter Chapter { get; } = chapter;
    public string? MessageText { get; private set; }

    public void Message(string text)
    {
        MessageText = text ?? "";
    }
}

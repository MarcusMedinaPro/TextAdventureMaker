// <copyright file="ChapterProgressRenderer.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using System.Text;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class ChapterProgressRenderer
{
    public string Render(IChapter chapter)
    {
        if (chapter == null)
        {
            return "No chapter active.";
        }

        List<IChapterObjective> objectives = chapter.Objectives.ToList();
        StringBuilder builder = new();
        _ = builder.AppendLine("╔══════════════════════════════════════╗");
        _ = builder.AppendLine($"║  {chapter.Title}{"".PadRight(Math.Max(0, 34 - chapter.Title.Length))}║");
        _ = builder.AppendLine("╠══════════════════════════════════════╣");

        foreach (IChapterObjective objective in objectives)
        {
            string marker = objective.IsComplete ? "[✓]" : "[ ]";
            _ = builder.AppendLine($"║  {marker} {objective.Id}".PadRight(38) + "║");
        }

        _ = builder.AppendLine("╚══════════════════════════════════════╝");
        return builder.ToString().TrimEnd();
    }
}

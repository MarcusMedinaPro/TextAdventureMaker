// <copyright file="JourneyProgressTracker.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using System.Text;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class JourneyProgressTracker
{
    public string Render(IHeroJourney journey, JourneyStage currentStage)
    {
        if (journey == null)
        {
            return "No journey tracked.";
        }

        List<JourneyStage> stages = journey.Stages.Select(stage => stage.Stage).ToList();
        int currentIndex = stages.IndexOf(currentStage);
        int total = stages.Count;
        int shownIndex = currentIndex >= 0 ? currentIndex + 1 : 0;

        StringBuilder builder = new();
        _ = builder.AppendLine("=== YOUR JOURNEY ===");
        _ = builder.AppendLine($"Stage: {shownIndex}/{total}");

        if (total > 0)
        {
            _ = builder.AppendLine();
            for (int i = 0; i < total; i++)
            {
                string marker = i < currentIndex ? "[✓]" : i == currentIndex ? "[●]" : "[ ]";
                _ = builder.Append(marker);
            }
            _ = builder.AppendLine();
        }

        return builder.ToString().TrimEnd();
    }
}

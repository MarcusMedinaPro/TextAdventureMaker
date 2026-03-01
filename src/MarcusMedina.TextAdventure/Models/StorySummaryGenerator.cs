// <copyright file="StorySummaryGenerator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using System.Text;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Generates narrative summaries of player journeys from history data.
/// </summary>
public sealed class StorySummaryGenerator
{
    /// <summary>
    /// Generates a formatted story summary from player history.
    /// </summary>
    public string Generate(IPlayerHistory history)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== Your Journey ===\n");

        // Statistics
        sb.AppendLine($"Time played: {history.GetTotalPlayTime():hh\\:mm\\:ss}");
        sb.AppendLine($"Commands entered: {history.GetCommandCount()}");
        sb.AppendLine($"Locations visited: {history.GetVisitedLocations().Count()}");
        sb.AppendLine($"NPCs met: {history.GetMetNpcs().Count()}");
        sb.AppendLine($"Items acquired: {history.GetAcquiredItems().Count()}");
        sb.AppendLine();

        // Key moments
        var keyMoments = history.Entries
            .Where(e => e.Type is HistoryEventType.QuestCompleted
                             or HistoryEventType.NpcKilled
                             or HistoryEventType.Achievement
                             or HistoryEventType.Death)
            .OrderBy(e => e.Turn);

        if (keyMoments.Any())
        {
            sb.AppendLine("Key moments:");
            foreach (var moment in keyMoments)
            {
                sb.AppendLine($"  Turn {moment.Turn}: {moment.Description}");
            }
        }

        return sb.ToString();
    }
}

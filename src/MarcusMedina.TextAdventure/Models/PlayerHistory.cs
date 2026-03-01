// <copyright file="PlayerHistory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Records and tracks player journey through the game.
/// </summary>
public sealed class PlayerHistory : IPlayerHistory
{
    private readonly List<HistoryEntry> _entries = [];
    private readonly DateTime _startTime = DateTime.Now;
    private int _turn;

    public IReadOnlyList<HistoryEntry> Entries => _entries.AsReadOnly();

    /// <summary>
    /// Records an event in history.
    /// </summary>
    public void Record(HistoryEventType type, object? data)
    {
        var description = GenerateDescription(type, data);
        _entries.Add(new HistoryEntry(DateTime.Now, _turn, type, description, data));
    }

    /// <summary>
    /// Increments the turn counter (called after each command).
    /// </summary>
    public void IncrementTurn() => _turn++;

    /// <summary>
    /// Gets all entries of a specific type.
    /// </summary>
    public IEnumerable<HistoryEntry> GetByType(HistoryEventType type) =>
        _entries.Where(e => e.Type == type);

    /// <summary>
    /// Gets all unique visited locations in order.
    /// </summary>
    public IEnumerable<ILocation> GetVisitedLocations() =>
        _entries
            .Where(e => e.Type == HistoryEventType.LocationVisited)
            .Select(e => e.Data)
            .OfType<ILocation>()
            .Distinct();

    /// <summary>
    /// Gets all unique NPCs met in order.
    /// </summary>
    public IEnumerable<INpc> GetMetNpcs() =>
        _entries
            .Where(e => e.Type == HistoryEventType.NpcMet)
            .Select(e => e.Data)
            .OfType<INpc>()
            .Distinct();

    /// <summary>
    /// Gets all items acquired in order.
    /// </summary>
    public IEnumerable<IItem> GetAcquiredItems() =>
        _entries
            .Where(e => e.Type == HistoryEventType.ItemAcquired)
            .Select(e => e.Data)
            .OfType<IItem>();

    /// <summary>
    /// Gets total time played.
    /// </summary>
    public TimeSpan GetTotalPlayTime() => DateTime.Now - _startTime;

    /// <summary>
    /// Gets total commands entered (same as turn count).
    /// </summary>
    public int GetCommandCount() => _turn;

    /// <summary>
    /// Generates a narrative summary of the journey.
    /// </summary>
    public string GenerateSummary()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== Your Journey ===\n");

        // Statistics
        sb.AppendLine($"Time played: {GetTotalPlayTime():hh\\:mm\\:ss}");
        sb.AppendLine($"Commands entered: {GetCommandCount()}");
        sb.AppendLine($"Locations visited: {GetVisitedLocations().Count()}");
        sb.AppendLine($"NPCs met: {GetMetNpcs().Count()}");
        sb.AppendLine($"Items acquired: {GetAcquiredItems().Count()}");
        sb.AppendLine();

        // Key moments
        var keyMoments = _entries
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

    private static string GenerateDescription(HistoryEventType type, object? data) =>
        type switch
        {
            HistoryEventType.LocationVisited when data is ILocation loc =>
                $"Visited {loc.Id}",
            HistoryEventType.ItemAcquired when data is IItem item =>
                $"Acquired {item.Name}",
            HistoryEventType.ItemDropped when data is IItem item =>
                $"Dropped {item.Name}",
            HistoryEventType.ItemUsed when data is IItem item =>
                $"Used {item.Name}",
            HistoryEventType.NpcMet when data is INpc npc =>
                $"Met {npc.Name}",
            HistoryEventType.NpcTalked when data is INpc npc =>
                $"Talked to {npc.Name}",
            HistoryEventType.NpcKilled when data is INpc npc =>
                $"Defeated {npc.Name}",
            HistoryEventType.QuestStarted when data is IQuest quest =>
                $"Started quest: {quest.Title}",
            HistoryEventType.QuestCompleted when data is IQuest quest =>
                $"Completed quest: {quest.Title}",
            HistoryEventType.QuestFailed when data is IQuest quest =>
                $"Failed quest: {quest.Title}",
            HistoryEventType.CombatStarted =>
                "Entered combat",
            HistoryEventType.CombatEnded =>
                "Exited combat",
            HistoryEventType.Achievement =>
                $"Achievement: {data}",
            HistoryEventType.Death =>
                "Died",
            _ => data?.ToString() ?? type.ToString()
        };
}

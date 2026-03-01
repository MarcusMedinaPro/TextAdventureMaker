// <copyright file="IPlayerHistory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Event types recorded in player history for analytics and achievements.
/// </summary>
public enum HistoryEventType
{
    LocationVisited,
    ItemAcquired,
    ItemDropped,
    ItemUsed,
    NpcMet,
    NpcTalked,
    NpcKilled,
    QuestStarted,
    QuestCompleted,
    QuestFailed,
    CombatStarted,
    CombatEnded,
    DoorOpened,
    DoorLocked,
    Achievement,
    Death,
    Custom
}

/// <summary>
/// Single entry in player history.
/// </summary>
public sealed record HistoryEntry(
    DateTime Timestamp,
    int Turn,
    HistoryEventType Type,
    string Description,
    object? Data
);

/// <summary>
/// Tracks complete player journey through the game.
/// Records events, provides analytics, enables achievements.
/// </summary>
public interface IPlayerHistory
{
    /// <summary>
    /// All recorded history entries.
    /// </summary>
    IReadOnlyList<HistoryEntry> Entries { get; }

    /// <summary>
    /// Records an event in the history.
    /// </summary>
    void Record(HistoryEventType type, object? data);

    /// <summary>
    /// Gets all entries of a specific type.
    /// </summary>
    IEnumerable<HistoryEntry> GetByType(HistoryEventType type);

    /// <summary>
    /// Gets all visited locations in order.
    /// </summary>
    IEnumerable<ILocation> GetVisitedLocations();

    /// <summary>
    /// Gets all NPCs met in order.
    /// </summary>
    IEnumerable<INpc> GetMetNpcs();

    /// <summary>
    /// Gets all items acquired in order.
    /// </summary>
    IEnumerable<IItem> GetAcquiredItems();

    /// <summary>
    /// Gets total play time elapsed.
    /// </summary>
    TimeSpan GetTotalPlayTime();

    /// <summary>
    /// Gets total number of commands entered.
    /// </summary>
    int GetCommandCount();

    /// <summary>
    /// Generates a story summary of the journey.
    /// </summary>
    string GenerateSummary();
}

// <copyright file="INpcMemory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Types of memories an NPC can have with different persistence.
/// </summary>
public enum MemoryType
{
    ShortTerm,    // Fades after several turns
    LongTerm,     // Persistent across sessions
    Emotional     // Affects relationships
}

/// <summary>
/// Represents a single memory entry for an NPC.
/// </summary>
public sealed record MemoryEntry(
    string Key,
    object Value,
    MemoryType Type,
    DateTime Timestamp,
    int Importance  // 1-10
);

/// <summary>
/// Interface for NPC memory system that tracks interactions and experiences.
/// </summary>
public interface INpcMemory
{
    /// <summary>
    /// Records a memory in the NPC's mind.
    /// </summary>
    void Remember(string key, object value, MemoryType type = MemoryType.ShortTerm);

    /// <summary>
    /// Recalls a memory by key.
    /// </summary>
    T? Recall<T>(string key);

    /// <summary>
    /// Checks if the NPC knows about something.
    /// </summary>
    bool Knows(string key);

    /// <summary>
    /// Forgets a specific memory.
    /// </summary>
    void Forget(string key);

    /// <summary>
    /// Gets the most recent memories.
    /// </summary>
    IEnumerable<MemoryEntry> GetRecentMemories(int count = 10);

    /// <summary>
    /// Gets all memories about a specific subject.
    /// </summary>
    IEnumerable<MemoryEntry> GetMemoriesAbout(string subject);
}

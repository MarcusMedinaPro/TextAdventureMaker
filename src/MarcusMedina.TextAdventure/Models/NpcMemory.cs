// <copyright file="NpcMemory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Manages an NPC's memories with support for different memory types and decay.
/// </summary>
public sealed class NpcMemory : INpcMemory
{
    // Legacy support
    private readonly Dictionary<string, int> _counters = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _knowledge = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _said = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, MemoryEntry> _memories = [];
    private readonly int _shortTermCapacity = 20;
    public bool HasMet { get; private set; }

    public string? Get(string key) => _knowledge.TryGetValue(key, out var value) ? value : null;

    public int GetCounter(string key) => _counters.TryGetValue(key, out var value) ? value : 0;

    public bool HasSaid(string id) => _said.Contains(id);

    public int Increment(string key, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return 0;
        }

        var next = GetCounter(key) + amount;
        _counters[key] = next;
        return next;
    }

    public bool Knows(string key) => _knowledge.ContainsKey(key);

    public void MarkMet() => HasMet = true;

    public void MarkSaid(string id)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            _ = _said.Add(id);
        }
    }

    public void Remember(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        _knowledge[key] = value ?? "";
    }

    // INpcMemory implementation
    public void Remember(string key, object value, MemoryType type = MemoryType.ShortTerm)
    {
        var entry = new MemoryEntry(key, value, type, DateTime.Now, CalculateImportance(key, value));
        _memories[key] = entry;

        if (type == MemoryType.ShortTerm)
            CleanupShortTermMemories();
    }

    public T? Recall<T>(string key) =>
        _memories.TryGetValue(key, out var entry) ? (T?)entry.Value : default;

    public void Forget(string key) =>
        _memories.Remove(key);

    public IEnumerable<MemoryEntry> GetRecentMemories(int count = 10) =>
        _memories.Values
            .OrderByDescending(m => m.Timestamp)
            .Take(count);

    public IEnumerable<MemoryEntry> GetMemoriesAbout(string subject) =>
        _memories.Values
            .Where(m => m.Key.Contains(subject, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Called each game turn to allow short-term memories to decay.
    /// </summary>
    public void OnTick()
    {
        var toForget = _memories
            .Where(m => m.Value.Type == MemoryType.ShortTerm)
            .Where(m => (DateTime.Now - m.Value.Timestamp).TotalMinutes > 5)
            .Select(m => m.Key)
            .ToList();

        foreach (var key in toForget)
            _memories.Remove(key);
    }

    private void CleanupShortTermMemories()
    {
        var shortTerm = _memories
            .Where(m => m.Value.Type == MemoryType.ShortTerm)
            .OrderBy(m => m.Value.Importance)
            .ThenBy(m => m.Value.Timestamp)
            .ToList();

        while (shortTerm.Count > _shortTermCapacity)
        {
            _memories.Remove(shortTerm[0].Key);
            shortTerm.RemoveAt(0);
        }
    }

    private static int CalculateImportance(string key, object value) => key.Length % 10 + 1;
}
// <copyright file="WorldState.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public sealed class WorldState : IWorldState
{
    private readonly Dictionary<string, bool> _flags = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _counters = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _relationships = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _timeline = [];

    public IReadOnlyList<string> Timeline => _timeline;

    public bool GetFlag(string key) => !string.IsNullOrWhiteSpace(key) && _flags.TryGetValue(key, out var value) && value;

    public void SetFlag(string key, bool value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;
        _flags[key] = value;
    }

    public int GetCounter(string key) => string.IsNullOrWhiteSpace(key) ? 0 : _counters.TryGetValue(key, out var value) ? value : 0;

    public int Increment(string key, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(key))
            return 0;
        var next = GetCounter(key) + amount;
        _counters[key] = next;
        return next;
    }

    public int GetRelationship(string npcId) => string.IsNullOrWhiteSpace(npcId) ? 0 : _relationships.TryGetValue(npcId, out var value) ? value : 0;

    public void SetRelationship(string npcId, int value)
    {
        if (string.IsNullOrWhiteSpace(npcId))
            return;
        _relationships[npcId] = value;
    }

    public void AddTimeline(string entry)
    {
        if (string.IsNullOrWhiteSpace(entry))
            return;
        _timeline.Add(entry);
    }

    public IReadOnlyDictionary<string, bool> GetFlagsSnapshot() => new Dictionary<string, bool>(_flags, StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, int> GetCountersSnapshot() => new Dictionary<string, int>(_counters, StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, int> GetRelationshipsSnapshot() => new Dictionary<string, int>(_relationships, StringComparer.OrdinalIgnoreCase);

    public void Apply(
        IDictionary<string, bool>? flags,
        IDictionary<string, int>? counters,
        IDictionary<string, int>? relationships,
        IEnumerable<string>? timeline)
    {
        _flags.Clear();
        _counters.Clear();
        _relationships.Clear();
        _timeline.Clear();

        if (flags != null)
        {
            foreach (var entry in flags)
            {
                _flags[entry.Key] = entry.Value;
            }
        }

        if (counters != null)
        {
            foreach (var entry in counters)
            {
                _counters[entry.Key] = entry.Value;
            }
        }

        if (relationships != null)
        {
            foreach (var entry in relationships)
            {
                _relationships[entry.Key] = entry.Value;
            }
        }

        if (timeline != null)
        {
            foreach (var entry in timeline)
            {
                if (!string.IsNullOrWhiteSpace(entry))
                {
                    _timeline.Add(entry);
                }
            }
        }
    }
}

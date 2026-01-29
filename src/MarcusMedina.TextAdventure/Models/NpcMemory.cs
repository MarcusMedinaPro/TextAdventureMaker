// <copyright file="NpcMemory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using System;
using System.Collections.Generic;

public sealed class NpcMemory
{
    private readonly HashSet<string> _said = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _knowledge = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _counters = new(StringComparer.OrdinalIgnoreCase);

    public bool HasMet { get; private set; }

    public void MarkMet() => HasMet = true;

    public bool HasSaid(string id) => _said.Contains(id);

    public void MarkSaid(string id)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            _ = _said.Add(id);
        }
    }

    public bool Knows(string key) => _knowledge.ContainsKey(key);

    public string? Get(string key) => _knowledge.TryGetValue(key, out var value) ? value : null;

    public void Remember(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;
        _knowledge[key] = value ?? "";
    }

    public int GetCounter(string key) => _counters.TryGetValue(key, out var value) ? value : 0;

    public int Increment(string key, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(key))
            return 0;
        var next = GetCounter(key) + amount;
        _counters[key] = next;
        return next;
    }
}

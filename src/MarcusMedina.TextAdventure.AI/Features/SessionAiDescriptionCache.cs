// <copyright file="SessionAiDescriptionCache.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Features;

public sealed class SessionAiDescriptionCache : IAiDescriptionCache
{
    private readonly Dictionary<string, string> _entries = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _gate = new();

    public bool TryGet(string key, out string description)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            description = string.Empty;
            return false;
        }

        lock (_gate)
            return _entries.TryGetValue(key, out description!);
    }

    public void Set(string key, string description)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(description))
            return;

        lock (_gate)
            _entries[key] = description;
    }

    public void Clear()
    {
        lock (_gate)
            _entries.Clear();
    }
}

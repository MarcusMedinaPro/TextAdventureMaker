// <copyright file="AiDebugTracker.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Diagnostics;

/// <summary>Records per-turn AI and cache events for optional debug output.</summary>
public sealed class AiDebugTracker(bool enabled = false)
{
    private readonly List<AiDebugEvent> _events = [];
    private readonly Lock _sync = new();

    public bool Enabled { get; private set; } = enabled;

    public void StartTurn()
    {
        lock (_sync)
            _events.Clear();
    }

    public void Record(string eventName, string payload)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            return;

        lock (_sync)
            _events.Add(new AiDebugEvent(eventName.Trim(), Compact(payload)));
    }

    public string Toggle()
    {
        Enabled = !Enabled;
        return Enabled ? "AI debug is now ON." : "AI debug is now OFF.";
    }

    public string Set(bool enabledState)
    {
        Enabled = enabledState;
        return enabledState ? "AI debug is now ON." : "AI debug is now OFF.";
    }

    public string Status() => Enabled ? "AI debug is ON." : "AI debug is OFF.";

    public IReadOnlyList<string> BuildTurnLines()
    {
        List<AiDebugEvent> snapshot;
        lock (_sync)
            snapshot = [.. _events];

        if (snapshot.Count == 0)
            return [];

        bool usedAi = snapshot.Any(static e => e.IsAiUseEvent);
        bool usedCache = snapshot.Any(static e => e.IsCacheHitEvent);
        List<string> lines = [];

        if (usedAi && usedCache)
            lines.Add("[AI, Cache]");
        else if (usedAi)
            lines.Add("[AI]");
        else if (usedCache)
            lines.Add("[Cache]");

        if (!Enabled)
            return lines;

        foreach (AiDebugEvent entry in snapshot)
        {
            string? line = entry.Name switch
            {
                "parser.ai.call" or "feature.ai.call" => $"[AI call: {entry.Payload}]",
                "parser.ai.response" or "feature.ai.response" => $"[AI: {entry.Payload}]",
                "description.cache.hit" => $"[Cache: {entry.Payload}]",
                _ => null
            };

            if (!string.IsNullOrWhiteSpace(line))
                lines.Add(line);
        }

        return lines;
    }

    private static string Compact(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "(empty)";

        const int max = 220;
        string singleLine = value.ReplaceLineEndings(" ").Trim();
        return singleLine.Length <= max ? singleLine : $"{singleLine[..max]}...";
    }

    private sealed record AiDebugEvent(string Name, string Payload)
    {
        public bool IsAiUseEvent => Name is "parser.ai.call" or "feature.ai.call";
        public bool IsCacheHitEvent => Name == "description.cache.hit";
    }
}

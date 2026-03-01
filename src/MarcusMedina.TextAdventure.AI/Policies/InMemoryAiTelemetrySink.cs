// <copyright file="InMemoryAiTelemetrySink.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Concurrent;

namespace MarcusMedina.TextAdventure.AI.Policies;

public sealed class InMemoryAiTelemetrySink : IAiTelemetrySink
{
    private readonly ConcurrentDictionary<string, int> _eventCounts = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, int> _providerCounts = new(StringComparer.OrdinalIgnoreCase);
    private int _totalTokens;

    public int TotalTokens => Math.Max(0, _totalTokens);

    public IReadOnlyDictionary<string, int> EventCounts => _eventCounts;
    public IReadOnlyDictionary<string, int> ProviderCounts => _providerCounts;

    public void Record(AiTelemetryEvent telemetryEvent)
    {
        ArgumentNullException.ThrowIfNull(telemetryEvent);

        _ = _eventCounts.AddOrUpdate(telemetryEvent.EventName, 1, (_, current) => current + 1);
        _ = _providerCounts.AddOrUpdate(telemetryEvent.ProviderName, 1, (_, current) => current + 1);

        if (telemetryEvent.TotalTokens is int tokens && tokens > 0)
            _ = Interlocked.Add(ref _totalTokens, tokens);
    }
}

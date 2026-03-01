// <copyright file="AiTelemetryEvent.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed record AiTelemetryEvent(
    string EventName,
    string ProviderName,
    AiAttemptOutcome Outcome,
    int? TotalTokens = null,
    DateTimeOffset? TimestampUtc = null)
{
    public DateTimeOffset AtUtc { get; } = TimestampUtc ?? DateTimeOffset.UtcNow;
}

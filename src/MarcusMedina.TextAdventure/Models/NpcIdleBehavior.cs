// <copyright file="NpcIdleBehavior.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Ambient idle behaviour for an NPC: every <see cref="Interval"/> commands a random message is shown.
/// Each instance owns its own step counter so multiple NPCs (or multiple behaviours per NPC) track independently.
/// </summary>
public sealed class NpcIdleBehavior(int interval, string[] messages)
{
    private int _stepCount;

    public int Interval { get; } = interval > 0 ? interval : throw new ArgumentOutOfRangeException(nameof(interval), "Interval must be > 0.");
    public string[] Messages { get; } = messages is { Length: > 0 } ? messages : throw new ArgumentException("At least one message required.", nameof(messages));

    /// <summary>
    /// Increments the step counter. Returns a random message when the counter hits a multiple of <see cref="Interval"/>,
    /// otherwise returns <c>null</c>.
    /// </summary>
    public string? Tick()
    {
        _stepCount++;
        return _stepCount % Interval == 0
            ? Messages[Random.Shared.Next(Messages.Length)]
            : null;
    }
}

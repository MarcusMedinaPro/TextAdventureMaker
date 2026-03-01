// <copyright file="TimedSpawn.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class TimedSpawn(string itemId) : ITimedSpawn
{
    private readonly List<int> _appearTicks = [];
    private readonly List<TimePhase> _appearPhases = [];
    private readonly List<Func<IGameState, bool>> _appearConditions = [];
    private readonly List<int> _disappearAfterTicks = [];
    private readonly List<TimePhase> _disappearPhases = [];

    public string ItemId { get; } = itemId ?? "";
    public string? MessageText { get; private set; }

    public IReadOnlyCollection<int> AppearTicks => _appearTicks;
    public IReadOnlyCollection<TimePhase> AppearPhases => _appearPhases;
    public IReadOnlyCollection<Func<IGameState, bool>> AppearConditions => _appearConditions;
    public IReadOnlyCollection<int> DisappearAfterTicks => _disappearAfterTicks;
    public IReadOnlyCollection<TimePhase> DisappearPhases => _disappearPhases;

    public ITimedSpawn AppearsAt(int tick)
    {
        if (tick >= 0)
        {
            _appearTicks.Add(tick);
        }

        return this;
    }

    public ITimedSpawn AppearsAt(TimePhase phase)
    {
        _appearPhases.Add(phase);
        return this;
    }

    public ITimedSpawn AppearsWhen(Func<IGameState, bool> predicate)
    {
        if (predicate != null)
        {
            _appearConditions.Add(predicate);
        }

        return this;
    }

    public ITimedSpawn DisappearsAfter(int ticks)
    {
        if (ticks > 0)
        {
            _disappearAfterTicks.Add(ticks);
        }

        return this;
    }

    public ITimedSpawn DisappearsAt(TimePhase phase)
    {
        _disappearPhases.Add(phase);
        return this;
    }

    public ITimedSpawn Message(string text)
    {
        MessageText = text ?? "";
        return this;
    }

    public ITimedSpawn Or()
    {
        return this;
    }
}

// <copyright file="TimeSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class TimeSystem : ITimeSystem
{
    private readonly Dictionary<TimeOfDay, List<Action<IGameState>>> _phaseHandlers = new();
    private readonly Dictionary<int, List<Action<IGameState>>> _movesRemainingHandlers = new();
    private readonly List<Action<IGameState>> _movesExhaustedHandlers = new();
    private readonly List<TimedChallenge> _challenges = new();
    private readonly HashSet<int> _movesRemainingFired = new();
    private bool _movesExhaustedFired;
    private TimeOfDay _startTime = TimeOfDay.Dawn;

    public bool Enabled { get; private set; }
    public int CurrentTick { get; private set; }
    public int CurrentDay { get; private set; } = 1;
    public int TicksPerDay { get; private set; } = 100;
    public TimeOfDay CurrentTimeOfDay { get; private set; } = TimeOfDay.Dawn;
    public TimePhase CurrentPhase => (TimePhase)CurrentTimeOfDay;
    public int? MaxMoves { get; private set; }
    public int MovesUsed { get; private set; }
    public int? MovesRemaining => MaxMoves.HasValue ? Math.Max(0, MaxMoves.Value - MovesUsed) : null;

    public ITimeSystem Enable()
    {
        Enabled = true;
        return this;
    }

    public ITimeSystem SetStartTime(TimeOfDay startTime)
    {
        _startTime = startTime;
        CurrentTick = GetPhaseStartTick(startTime);
        CurrentTimeOfDay = startTime;
        return this;
    }

    public ITimeSystem SetTicksPerDay(int ticksPerDay)
    {
        TicksPerDay = Math.Max(4, ticksPerDay);
        CurrentTick = GetPhaseStartTick(_startTime);
        CurrentTimeOfDay = _startTime;
        return this;
    }

    public ITimeSystem SetMaxMoves(int maxMoves)
    {
        MaxMoves = Math.Max(1, maxMoves);
        _movesRemainingFired.Clear();
        _movesExhaustedFired = false;
        return this;
    }

    public ITimeSystem OnPhase(TimeOfDay phase, Action<IGameState> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        if (!_phaseHandlers.TryGetValue(phase, out var handlers))
        {
            handlers = new List<Action<IGameState>>();
            _phaseHandlers[phase] = handlers;
        }
        handlers.Add(handler);
        return this;
    }

    public ITimeSystem OnMovesRemaining(int movesRemaining, Action<IGameState> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        var key = Math.Max(0, movesRemaining);
        if (!_movesRemainingHandlers.TryGetValue(key, out var handlers))
        {
            handlers = new List<Action<IGameState>>();
            _movesRemainingHandlers[key] = handlers;
        }
        handlers.Add(handler);
        return this;
    }

    public ITimeSystem OnMovesExhausted(Action<IGameState> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _movesExhaustedHandlers.Add(handler);
        return this;
    }

    public ITimedChallenge CreateTimedChallenge(string id)
    {
        var challenge = new TimedChallenge(id);
        _challenges.Add(challenge);
        return challenge;
    }

    public void Tick(IGameState state)
    {
        if (!Enabled) return;
        ArgumentNullException.ThrowIfNull(state);

        MovesUsed++;

        if (MaxMoves.HasValue)
        {
            var remaining = MovesRemaining ?? 0;
            if (_movesRemainingHandlers.TryGetValue(remaining, out var handlers) && !_movesRemainingFired.Contains(remaining))
            {
                _movesRemainingFired.Add(remaining);
                foreach (var handler in handlers)
                {
                    handler(state);
                }
            }

            if (remaining <= 0 && !_movesExhaustedFired)
            {
                _movesExhaustedFired = true;
                foreach (var handler in _movesExhaustedHandlers)
                {
                    handler(state);
                }
            }
        }

        CurrentTick++;
        var tickInDay = CurrentTick % TicksPerDay;
        if (tickInDay == 0)
        {
            CurrentDay++;
        }

        var nextPhase = GetPhaseFromTick(tickInDay, TicksPerDay);
        if (nextPhase != CurrentTimeOfDay)
        {
            CurrentTimeOfDay = nextPhase;
            if (_phaseHandlers.TryGetValue(nextPhase, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler(state);
                }
            }
        }

        foreach (var challenge in _challenges)
        {
            challenge.Tick(state);
        }
    }

    private int GetPhaseStartTick(TimeOfDay phase)
    {
        return phase switch
        {
            TimeOfDay.Dawn => 0,
            TimeOfDay.Day => TicksPerDay / 4,
            TimeOfDay.Dusk => TicksPerDay / 2,
            TimeOfDay.Night => (TicksPerDay * 3) / 4,
            _ => 0
        };
    }

    private static TimeOfDay GetPhaseFromTick(int tickInDay, int ticksPerDay)
    {
        var quarter = ticksPerDay / 4.0;
        if (tickInDay < quarter) return TimeOfDay.Dawn;
        if (tickInDay < quarter * 2) return TimeOfDay.Day;
        if (tickInDay < quarter * 3) return TimeOfDay.Dusk;
        return TimeOfDay.Night;
    }
}

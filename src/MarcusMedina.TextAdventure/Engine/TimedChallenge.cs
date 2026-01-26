// <copyright file="TimedChallenge.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class TimedChallenge : ITimedChallenge
{
    private readonly Dictionary<int, List<Action<IGameState>>> _movesRemainingHandlers = new();
    private readonly List<Action<IGameState>> _startHandlers = new();
    private readonly List<Action<IGameState>> _successHandlers = new();
    private readonly List<Action<IGameState>> _failureHandlers = new();
    private bool _exhaustedFired;
    private readonly HashSet<int> _movesRemainingFired = new();

    public string Id { get; }
    public int MaxMoves { get; private set; } = 10;
    public int MovesUsed { get; private set; }
    public int MovesRemaining => Math.Max(0, MaxMoves - MovesUsed);
    public bool IsActive { get; private set; }

    public TimedChallenge(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
    }

    public ITimedChallenge MaxMovesLimit(int maxMoves)
    {
        MaxMoves = Math.Max(1, maxMoves);
        return this;
    }

    public ITimedChallenge OnStart(Action<IGameState> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _startHandlers.Add(handler);
        return this;
    }

    public ITimedChallenge OnMovesRemaining(int movesRemaining, Action<IGameState> handler)
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

    public ITimedChallenge OnSuccess(Action<IGameState> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _successHandlers.Add(handler);
        return this;
    }

    public ITimedChallenge OnFailure(Action<IGameState> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _failureHandlers.Add(handler);
        return this;
    }

    public void Start(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        IsActive = true;
        MovesUsed = 0;
        _exhaustedFired = false;
        _movesRemainingFired.Clear();
        foreach (var handler in _startHandlers)
        {
            handler(state);
        }
    }

    public void Succeed(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (!IsActive) return;
        IsActive = false;
        foreach (var handler in _successHandlers)
        {
            handler(state);
        }
    }

    public void Fail(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (!IsActive) return;
        IsActive = false;
        foreach (var handler in _failureHandlers)
        {
            handler(state);
        }
    }

    public void Tick(IGameState state)
    {
        if (!IsActive) return;
        MovesUsed++;

        if (_movesRemainingHandlers.TryGetValue(MovesRemaining, out var handlers) && !_movesRemainingFired.Contains(MovesRemaining))
        {
            _movesRemainingFired.Add(MovesRemaining);
            foreach (var handler in handlers)
            {
                handler(state);
            }
        }

        if (MovesRemaining <= 0 && !_exhaustedFired)
        {
            _exhaustedFired = true;
            Fail(state);
        }
    }
}

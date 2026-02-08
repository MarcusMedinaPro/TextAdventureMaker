// <copyright file="TimedDoor.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class TimedDoor(string doorId) : ITimedDoor
{
    private readonly List<int> _openTicks = [];
    private readonly List<int> _closeTicks = [];
    private readonly List<TimePhase> _openPhases = [];
    private readonly List<TimePhase> _closePhases = [];
    private readonly List<Func<IGameState, bool>> _openConditions = [];
    private readonly List<Func<IGameState, bool>> _closeConditions = [];
    private Func<IGameState, bool>? _permanentOpenCondition;

    public string DoorId { get; } = doorId ?? "";
    public string? MessageText { get; private set; }
    public string? ClosedMessageText { get; private set; }

    public IReadOnlyCollection<int> OpenTicks => _openTicks;
    public IReadOnlyCollection<int> CloseTicks => _closeTicks;
    public IReadOnlyCollection<TimePhase> OpenPhases => _openPhases;
    public IReadOnlyCollection<TimePhase> ClosePhases => _closePhases;
    public IReadOnlyCollection<Func<IGameState, bool>> OpenConditions => _openConditions;
    public IReadOnlyCollection<Func<IGameState, bool>> CloseConditions => _closeConditions;
    public Func<IGameState, bool>? PermanentOpenCondition => _permanentOpenCondition;

    public ITimedDoor OpensAt(int tick)
    {
        if (tick >= 0)
        {
            _openTicks.Add(tick);
        }

        return this;
    }

    public ITimedDoor ClosesAt(int tick)
    {
        if (tick >= 0)
        {
            _closeTicks.Add(tick);
        }

        return this;
    }

    public ITimedDoor OpensAt(TimePhase phase)
    {
        _openPhases.Add(phase);
        return this;
    }

    public ITimedDoor ClosesAt(TimePhase phase)
    {
        _closePhases.Add(phase);
        return this;
    }

    public ITimedDoor OpensWhen(Func<IGameState, bool> predicate)
    {
        if (predicate != null)
        {
            _openConditions.Add(predicate);
        }

        return this;
    }

    public ITimedDoor ClosesWhen(Func<IGameState, bool> predicate)
    {
        if (predicate != null)
        {
            _closeConditions.Add(predicate);
        }

        return this;
    }

    public ITimedDoor PermanentlyOpensWhen(Func<IGameState, bool> predicate)
    {
        _permanentOpenCondition = predicate;
        return this;
    }

    public ITimedDoor Message(string text)
    {
        MessageText = text ?? "";
        return this;
    }

    public ITimedDoor ClosedMessage(string text)
    {
        ClosedMessageText = text ?? "";
        return this;
    }

    public ITimedDoor Or()
    {
        return this;
    }
}

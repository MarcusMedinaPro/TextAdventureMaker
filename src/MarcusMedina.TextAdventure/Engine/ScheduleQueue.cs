// <copyright file="ScheduleQueue.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class ScheduleQueue : IScheduleQueue
{
    private readonly List<ScheduledEvent> _events = [];
    private readonly IGameState _state;

    public ScheduleQueue(IGameState state)
    {
        _state = state;
    }

    public IReadOnlyList<ScheduledEvent> Events => _events;

    public IScheduleQueue At(int tick, Action<ScheduleContext> handler)
    {
        _events.Add(new ScheduledEvent(tick, null, null, handler));
        return this;
    }

    public IScheduleQueue Every(int ticks, Action<ScheduleContext> handler)
    {
        _events.Add(new ScheduledEvent(null, ticks, null, handler));
        return this;
    }

    public IScheduleQueue When(Func<IGameState, bool> condition, Action<ScheduleContext> handler)
    {
        _events.Add(new ScheduledEvent(null, null, condition, handler));
        return this;
    }

    public void Trigger(int tick)
    {
        ScheduleContext context = new(_state);
        foreach (ScheduledEvent scheduled in _events)
        {
            if (scheduled.Matches(_state, tick))
            {
                scheduled.Handler(context);
            }
        }
    }
}

public sealed class ScheduledEvent(int? atTick, int? everyTicks, Func<IGameState, bool>? condition, Action<ScheduleContext> handler)
{
    public int? AtTick { get; } = atTick;
    public int? EveryTicks { get; } = everyTicks;
    public Func<IGameState, bool>? Condition { get; } = condition;
    public Action<ScheduleContext> Handler { get; } = handler;

    public bool Matches(IGameState state, int tick)
    {
        if (AtTick.HasValue && AtTick.Value == tick)
        {
            return true;
        }

        if (EveryTicks.HasValue && EveryTicks.Value > 0 && tick % EveryTicks.Value == 0)
        {
            return true;
        }

        return Condition != null && Condition(state);
    }
}

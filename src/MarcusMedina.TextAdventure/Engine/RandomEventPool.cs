// <copyright file="RandomEventPool.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Engine;

using MarcusMedina.TextAdventure.Interfaces;

public sealed class RandomEventPool : IRandomEventPool
{
    private readonly List<RandomEvent> _events = [];
    private readonly Random _random = new();
    private int _tick;

    public bool Enabled { get; private set; }
    public double TriggerChance { get; private set; } = 0.15;

    public IRandomEventPool Enable()
    {
        Enabled = true;
        return this;
    }

    public IRandomEventPool SetTriggerChance(double chance)
    {
        TriggerChance = Math.Clamp(chance, 0.0, 1.0);
        return this;
    }

    public IRandomEventPool AddEvent(string id, int weight, Action<IGameState> handler, Func<IGameState, bool>? condition = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Event id cannot be empty.", nameof(id));
        }

        ArgumentNullException.ThrowIfNull(handler);

        _events.Add(new RandomEvent(id, Math.Max(1, weight), handler, condition));
        return this;
    }

    public IRandomEventPool SetCooldown(string id, int cooldownTicks)
    {
        var ev = _events.FirstOrDefault(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        _ = ev?.CooldownTicks = Math.Max(0, cooldownTicks);
        return this;
    }

    public void Tick(IGameState state)
    {
        if (!Enabled)
            return;
        ArgumentNullException.ThrowIfNull(state);

        if (_random.NextDouble() > TriggerChance)
            return;

        var now = state.TimeSystem.Enabled ? state.TimeSystem.CurrentTick : ++_tick;
        var candidates = _events
            .Where(e => e.CanTrigger(state, now))
            .ToList();

        if (candidates.Count == 0)
            return;

        var totalWeight = candidates.Sum(e => e.Weight);
        var roll = _random.Next(0, totalWeight);
        var cursor = 0;
        foreach (var ev in candidates)
        {
            cursor += ev.Weight;
            if (roll < cursor)
            {
                ev.Trigger(state, now);
                break;
            }
        }
    }

    private sealed class RandomEvent(string id, int weight, Action<IGameState> handler, Func<IGameState, bool>? condition)
    {
        public string Id { get; } = id;
        public int Weight { get; } = weight;
        public int CooldownTicks { get; set; }
        public int LastTriggeredTick { get; private set; } = int.MinValue;
        public Action<IGameState> Handler { get; } = handler;
        public Func<IGameState, bool>? Condition { get; } = condition;

        public bool CanTrigger(IGameState state, int now) => (CooldownTicks <= 0 || (long)now - LastTriggeredTick >= CooldownTicks) && (Condition == null || Condition(state));

        public void Trigger(IGameState state, int now)
        {
            LastTriggeredTick = now;
            Handler(state);
        }
    }
}

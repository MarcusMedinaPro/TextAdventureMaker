// <copyright file="TensionSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class TensionSystem : ITensionSystem
{
    private sealed record TensionModifier(string Id, Func<IGameState, float> Modifier, int MinTicksBetween, int LastTick);
    private sealed record RestZone(Func<IGameState, bool> Predicate, float Decay);

    private readonly List<TensionModifier> _modifiers = [];
    private readonly List<RestZone> _restZones = [];
    private int _lastMajorEventTick = -1;

    public float Current { get; private set; }

    public ITensionSystem Set(float value)
    {
        Current = Math.Clamp(value, 0f, 1f);
        return this;
    }

    public ITensionSystem Modify(float delta)
    {
        return Set(Current + delta);
    }

    public ITensionSystem AddModifier(string id, Func<IGameState, float> modifier, int minTicksBetween = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(modifier);
        _modifiers.Add(new TensionModifier(id, modifier, Math.Max(0, minTicksBetween), LastTick: -1));
        return this;
    }

    public ITensionSystem SetRestZone(Func<IGameState, bool> predicate, float decayPerTick = 0.05f)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        _restZones.Add(new RestZone(predicate, Math.Abs(decayPerTick)));
        return this;
    }

    public bool CanTriggerMajorEvent(IGameState state, int minTicksBetween)
    {
        ArgumentNullException.ThrowIfNull(state);
        int now = state.TimeSystem.CurrentTick;
        return _lastMajorEventTick < 0 || now - _lastMajorEventTick >= Math.Max(0, minTicksBetween);
    }

    public void RegisterMajorEvent(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        _lastMajorEventTick = state.TimeSystem.CurrentTick;
    }

    public float GetEncounterWeight(float baseChance = 1f)
    {
        return Math.Max(0f, baseChance) * (0.5f + Current);
    }

    public float GetMusicIntensity()
    {
        return Current;
    }

    public void Tick(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        int now = state.TimeSystem.CurrentTick;
        for (int i = 0; i < _modifiers.Count; i++)
        {
            TensionModifier modifier = _modifiers[i];
            if (modifier.MinTicksBetween > 0 && modifier.LastTick >= 0 && now - modifier.LastTick < modifier.MinTicksBetween)
            {
                continue;
            }

            float delta = modifier.Modifier(state);
            if (Math.Abs(delta) > 0.0001f)
            {
                Modify(delta);
                _modifiers[i] = modifier with { LastTick = now };
            }
        }

        foreach (RestZone rest in _restZones)
        {
            if (rest.Predicate(state))
            {
                Modify(-rest.Decay);
            }
        }
    }
}

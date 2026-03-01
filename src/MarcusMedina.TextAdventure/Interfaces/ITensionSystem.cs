// <copyright file="ITensionSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ITensionSystem
{
    float Current { get; }
    ITensionSystem Set(float value);
    ITensionSystem Modify(float delta);
    ITensionSystem AddModifier(string id, Func<IGameState, float> modifier, int minTicksBetween = 0);
    ITensionSystem SetRestZone(Func<IGameState, bool> predicate, float decayPerTick = 0.05f);
    bool CanTriggerMajorEvent(IGameState state, int minTicksBetween);
    void RegisterMajorEvent(IGameState state);
    float GetEncounterWeight(float baseChance = 1f);
    float GetMusicIntensity();
    void Tick(IGameState state);
}

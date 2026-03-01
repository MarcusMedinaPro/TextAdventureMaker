// <copyright file="IRandomEventPool.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

public interface IRandomEventPool
{
    bool Enabled { get; }
    double TriggerChance { get; }

    IRandomEventPool AddEvent(string id, int weight, Action<IGameState> handler, Func<IGameState, bool>? condition = null);

    IRandomEventPool Enable();

    IRandomEventPool SetCooldown(string id, int cooldownTicks);

    IRandomEventPool SetTriggerChance(double chance);

    void Tick(IGameState state);
}
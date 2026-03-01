// <copyright file="PoisonEffect.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

public sealed class PoisonEffect(string sourceName, int damagePerTurn, int remainingTurns)
{
    public string SourceName { get; } = sourceName;
    public int DamagePerTurn { get; } = damagePerTurn;
    public int RemainingTurns { get; private set; } = remainingTurns;
    public bool IsExpired => RemainingTurns <= 0;

    public int Tick()
    {
        if (IsExpired)
            return 0;

        RemainingTurns--;
        return DamagePerTurn;
    }
}

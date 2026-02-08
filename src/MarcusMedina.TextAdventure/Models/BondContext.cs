// <copyright file="BondContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class BondContext(IGameState state, INpc npc, IBond bond)
{
    public IGameState State { get; } = state;
    public INpc Npc { get; } = npc;
    public IBond Bond { get; } = bond;
    public int ImpactWeight { get; set; }
}

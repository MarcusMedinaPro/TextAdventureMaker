// <copyright file="ICombatSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ICombatSystem
{
    CommandResult Attack(IGameState state, INpc target);
    CommandResult Flee(IGameState state, INpc? target = null);
}

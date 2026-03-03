// <copyright file="NpcReaction.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Defines a text reaction an NPC emits when a specific command trigger fires in their room.
/// </summary>
/// <param name="Trigger">Trigger string (e.g. "blow" or "blow:trumpet").</param>
/// <param name="Text">Message displayed when the reaction fires.</param>
/// <param name="Condition">Optional condition; reaction skipped when it returns false.</param>
/// <param name="EndGame">When true the game ends after displaying the reaction text.</param>
/// <param name="Effect">Optional side-effect (set flag, increment counter, etc.) applied when reaction fires.</param>
public sealed record NpcReaction(
    string Trigger,
    string Text,
    Func<IGameState, bool>? Condition = null,
    bool EndGame = false,
    Action<IGameState>? Effect = null);

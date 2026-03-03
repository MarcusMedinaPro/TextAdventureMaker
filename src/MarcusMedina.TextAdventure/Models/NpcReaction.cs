// <copyright file="NpcReaction.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Defines a text reaction an NPC emits when a specific command trigger fires in their room.
/// </summary>
public sealed record NpcReaction(string Trigger, string Text, Func<IGameState, bool>? Condition = null);

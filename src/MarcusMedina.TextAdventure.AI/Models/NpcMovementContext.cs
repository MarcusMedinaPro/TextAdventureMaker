// <copyright file="NpcMovementContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed record NpcMovementContext(
    string NpcId,
    string CurrentLocationId,
    IReadOnlyList<string> ReachableLocationIds,
    IReadOnlyList<string> Goals,
    string? PlayerLocationId = null);

// <copyright file="NpcMovementDecision.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed record NpcMovementDecision(
    string NextLocationId,
    string? Rationale = null,
    string? ProviderName = null,
    bool UsedFallback = false);

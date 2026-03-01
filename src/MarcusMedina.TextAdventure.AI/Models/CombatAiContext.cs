// <copyright file="CombatAiContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed record CombatAiContext(
    string NpcId,
    int NpcHealth,
    int PlayerHealth,
    IReadOnlyList<string> AvailableActionIds,
    string CurrentLocationId,
    IReadOnlyList<string>? EnvironmentTags = null);

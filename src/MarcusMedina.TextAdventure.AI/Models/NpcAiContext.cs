// <copyright file="NpcAiContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed record NpcAiContext(
    string NpcId,
    string NpcName,
    string Persona,
    string CurrentLocationId,
    IReadOnlyList<string> InventoryItems,
    NpcRelationshipSnapshot RelationshipToPlayer,
    IReadOnlyList<string>? KnownFacts = null);

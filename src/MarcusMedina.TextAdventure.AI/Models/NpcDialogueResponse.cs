// <copyright file="NpcDialogueResponse.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed record NpcDialogueResponse(
    string Reply,
    int RelationshipDelta = 0,
    string? ProviderName = null,
    bool UsedFallback = false);

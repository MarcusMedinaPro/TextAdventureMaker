// <copyright file="StoryEventProposal.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed record StoryEventProposal(
    string EventId,
    string Summary,
    string TargetLocationId,
    IReadOnlyList<string>? Consequences = null);

// <copyright file="AiProviderAttempt.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed record AiProviderAttempt(
    string ProviderName,
    AiAttemptOutcome Outcome,
    string? Message = null,
    int? TotalTokens = null);

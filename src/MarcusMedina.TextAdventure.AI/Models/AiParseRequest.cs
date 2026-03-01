// <copyright file="AiParseRequest.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed record AiParseRequest(
    string Input,
    string? SystemPrompt = null,
    string? Context = null,
    string? Language = null,
    int EstimatedTokens = 128);

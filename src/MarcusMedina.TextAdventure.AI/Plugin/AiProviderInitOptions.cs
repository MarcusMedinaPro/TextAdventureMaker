// <copyright file="AiProviderInitOptions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Plugin;

public sealed record AiProviderInitOptions
{
    public required AiProviderKind Provider { get; init; }
    public string? ApiKey { get; init; }
    public string? Model { get; init; }
    public string? Endpoint { get; init; }
    public string? SystemPrompt { get; init; }
    public int? TimeoutMs { get; init; }
    public double? Temperature { get; init; }
    public int? DailyTokenLimit { get; init; }
    public bool Enabled { get; init; } = true;
}

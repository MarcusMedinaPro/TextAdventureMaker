// <copyright file="AiRoutingResult.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed class AiRoutingResult
{
    public string? ProviderName { get; init; }
    public string? CommandText { get; init; }
    public AiTokenUsage? TokenUsage { get; init; }
    public required IReadOnlyList<AiProviderAttempt> Attempts { get; init; }
    public bool HasCommand => !string.IsNullOrWhiteSpace(CommandText);

    public static AiRoutingResult Success(string providerName, string commandText, IReadOnlyList<AiProviderAttempt> attempts, AiTokenUsage? tokenUsage = null)
    {
        return new AiRoutingResult
        {
            ProviderName = providerName,
            CommandText = commandText,
            Attempts = attempts,
            TokenUsage = tokenUsage
        };
    }

    public static AiRoutingResult Failure(IReadOnlyList<AiProviderAttempt> attempts)
    {
        return new AiRoutingResult
        {
            Attempts = attempts
        };
    }
}

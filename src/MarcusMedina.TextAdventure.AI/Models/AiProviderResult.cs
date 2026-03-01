// <copyright file="AiProviderResult.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Models;

public sealed class AiProviderResult
{
    public required string ProviderName { get; init; }
    public required AiAttemptOutcome Outcome { get; init; }
    public string? CommandText { get; init; }
    public string? Message { get; init; }
    public AiTokenUsage? TokenUsage { get; init; }
    public bool IsSuccess => Outcome == AiAttemptOutcome.Success && !string.IsNullOrWhiteSpace(CommandText);

    public static AiProviderResult Success(string providerName, string commandText, AiTokenUsage? tokenUsage = null)
    {
        return new AiProviderResult
        {
            ProviderName = providerName,
            Outcome = AiAttemptOutcome.Success,
            CommandText = commandText,
            TokenUsage = tokenUsage
        };
    }

    public static AiProviderResult InvalidOutput(string providerName, string? message = null)
    {
        return new AiProviderResult
        {
            ProviderName = providerName,
            Outcome = AiAttemptOutcome.InvalidOutput,
            Message = message
        };
    }

    public static AiProviderResult Failed(string providerName, string? message = null)
    {
        return new AiProviderResult
        {
            ProviderName = providerName,
            Outcome = AiAttemptOutcome.Failed,
            Message = message
        };
    }

    public static AiProviderResult Unavailable(string providerName, string? message = null)
    {
        return new AiProviderResult
        {
            ProviderName = providerName,
            Outcome = AiAttemptOutcome.SkippedUnavailable,
            Message = message
        };
    }
}

// <copyright file="AiProviderRouter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Router;

public sealed class AiProviderRouter : IAiProviderRouter
{
    private readonly IReadOnlyList<IAiCommandProvider> _providers;
    private readonly ITokenBudgetPolicy? _budgetPolicy;
    private readonly IAiTelemetrySink? _telemetrySink;

    public AiProviderRouter(
        IEnumerable<IAiCommandProvider> providers,
        ITokenBudgetPolicy? budgetPolicy = null,
        IAiTelemetrySink? telemetrySink = null)
    {
        _providers = providers?.ToList() ?? throw new ArgumentNullException(nameof(providers));
        _budgetPolicy = budgetPolicy;
        _telemetrySink = telemetrySink;
    }

    public async Task<AiRoutingResult> RouteAsync(AiParseRequest request, CancellationToken cancellationToken = default)
    {
        List<AiProviderAttempt> attempts = [];

        foreach (IAiCommandProvider provider in _providers)
        {
            if (_budgetPolicy != null && !_budgetPolicy.CanUse(provider.Name, request.EstimatedTokens))
            {
                attempts.Add(new AiProviderAttempt(provider.Name, AiAttemptOutcome.SkippedBudget, "Daily token budget exceeded."));
                RecordTelemetry("route.skip_budget", provider.Name, AiAttemptOutcome.SkippedBudget);
                continue;
            }

            try
            {
                AiProviderResult result = await provider.ParseAsync(request, cancellationToken).ConfigureAwait(false);
                attempts.Add(new AiProviderAttempt(result.ProviderName, result.Outcome, result.Message, result.TokenUsage?.TotalTokens));
                RecordTelemetry("route.attempt", result.ProviderName, result.Outcome, result.TokenUsage?.TotalTokens);

                if (!result.IsSuccess || string.IsNullOrWhiteSpace(result.CommandText))
                    continue;

                if (result.TokenUsage?.TotalTokens is int usedTokens && usedTokens > 0)
                    _budgetPolicy?.TrackUsage(result.ProviderName, usedTokens);

                return AiRoutingResult.Success(result.ProviderName, result.CommandText, attempts, result.TokenUsage);
            }
            catch (OperationCanceledException)
            {
                attempts.Add(new AiProviderAttempt(provider.Name, AiAttemptOutcome.Failed, "AI request timed out."));
                RecordTelemetry("route.exception", provider.Name, AiAttemptOutcome.Failed);
            }
            catch (Exception ex)
            {
                attempts.Add(new AiProviderAttempt(provider.Name, AiAttemptOutcome.Exception, ex.Message));
                RecordTelemetry("route.exception", provider.Name, AiAttemptOutcome.Exception);
            }
        }

        return AiRoutingResult.Failure(attempts);
    }

    private void RecordTelemetry(string eventName, string providerName, AiAttemptOutcome outcome, int? tokens = null)
    {
        _telemetrySink?.Record(new AiTelemetryEvent(eventName, providerName, outcome, tokens));
    }
}

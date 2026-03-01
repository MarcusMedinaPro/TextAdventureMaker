// <copyright file="NpcMovementAiService.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.AI.Features;

public sealed class NpcMovementAiService(
    IAiProviderRouter router,
    AiParserOptions? options = null) : AiFeatureServiceBase(router, options), INpcMovementAiService
{
    public async Task<NpcMovementDecision?> ChooseNextLocationAsync(NpcMovementContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.ReachableLocationIds.Count == 0)
            return new NpcMovementDecision(context.CurrentLocationId, "No reachable locations.", UsedFallback: true);

        string prompt = AiFeaturePrompts.BuildNpcMovementPrompt(context);
        AiRoutingResult? route = await TryRouteAsync(prompt, cancellationToken).ConfigureAwait(false);
        if (route is null || string.IsNullOrWhiteSpace(route.CommandText))
            return BuildFallback(context);

        IReadOnlyDictionary<string, string> parsed = AiStructuredTextParser.ParseSegments(route.CommandText);
        string? proposed = parsed.GetValueOrDefault("move") ?? parsed.GetValueOrDefault("location") ?? route.CommandText;

        if (string.IsNullOrWhiteSpace(proposed))
            return BuildFallback(context);

        string? match = context.ReachableLocationIds.FirstOrDefault(x => x.TextCompare(proposed));
        if (string.IsNullOrWhiteSpace(match))
            return BuildFallback(context);

        return new NpcMovementDecision(
            NextLocationId: match,
            Rationale: parsed.GetValueOrDefault("reason"),
            ProviderName: route.ProviderName,
            UsedFallback: false);
    }

    private static NpcMovementDecision BuildFallback(NpcMovementContext context)
    {
        return new NpcMovementDecision(
            NextLocationId: context.CurrentLocationId,
            Rationale: "Fallback movement: stay in current location.",
            UsedFallback: true);
    }
}

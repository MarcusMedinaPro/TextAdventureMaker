// <copyright file="NpcCombatAiService.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.AI.Features;

public sealed class NpcCombatAiService(
    IAiProviderRouter router,
    AiParserOptions? options = null) : AiFeatureServiceBase(router, options), INpcCombatAiService
{
    public async Task<NpcCombatDecision?> DecideActionAsync(CombatAiContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.AvailableActionIds.Count == 0)
            return null;

        string prompt = AiFeaturePrompts.BuildCombatPrompt(context);
        AiRoutingResult? route = await TryRouteAsync(prompt, cancellationToken).ConfigureAwait(false);
        if (route is null || string.IsNullOrWhiteSpace(route.CommandText))
            return BuildFallback(context);

        IReadOnlyDictionary<string, string> parsed = AiStructuredTextParser.ParseSegments(route.CommandText);
        string? action = parsed.GetValueOrDefault("action") ?? route.CommandText;
        if (string.IsNullOrWhiteSpace(action))
            return BuildFallback(context);

        string? safeAction = context.AvailableActionIds.FirstOrDefault(x => x.TextCompare(action));
        if (string.IsNullOrWhiteSpace(safeAction))
            return BuildFallback(context);

        return new NpcCombatDecision(
            ActionId: safeAction,
            TargetId: parsed.GetValueOrDefault("target"),
            Rationale: parsed.GetValueOrDefault("reason"));
    }

    private static NpcCombatDecision BuildFallback(CombatAiContext context)
    {
        string action = context.AvailableActionIds.FirstOrDefault(x => x.TextCompare("attack"))
            ?? context.AvailableActionIds[0];

        return new NpcCombatDecision(action, Rationale: "Fallback combat action.");
    }
}

// <copyright file="NpcDialogueAiService.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Features;

public sealed class NpcDialogueAiService(
    IAiProviderRouter router,
    AiParserOptions? options = null) : AiFeatureServiceBase(router, options), INpcDialogueAiService
{
    public async Task<NpcDialogueResponse?> GenerateReplyAsync(NpcAiContext context, string playerInput, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (string.IsNullOrWhiteSpace(playerInput))
            return new NpcDialogueResponse("I have nothing to say.", 0, UsedFallback: true);

        string prompt = AiFeaturePrompts.BuildNpcDialoguePrompt(context, playerInput);
        AiRoutingResult? route = await TryRouteAsync(prompt, cancellationToken).ConfigureAwait(false);

        if (route is null || !route.HasCommand || string.IsNullOrWhiteSpace(route.CommandText))
            return BuildFallback(context);

        IReadOnlyDictionary<string, string> parsed = AiStructuredTextParser.ParseSegments(route.CommandText);
        string reply = parsed.GetValueOrDefault("reply") ?? parsed.GetValueOrDefault("text") ?? route.CommandText;
        int delta = AiStructuredTextParser.ParseIntOrDefault(parsed.GetValueOrDefault("delta"), -5, 5, 0);

        if (string.IsNullOrWhiteSpace(reply))
            return BuildFallback(context);

        return new NpcDialogueResponse(reply.Trim(), delta, route.ProviderName, UsedFallback: false);
    }

    private static NpcDialogueResponse BuildFallback(NpcAiContext context)
    {
        string reply = context.RelationshipToPlayer.Affinity switch
        {
            >= 50 => "You have my ear, friend.",
            <= -50 => "I do not trust you.",
            _ => "Speak plainly."
        };

        return new NpcDialogueResponse(reply, 0, UsedFallback: true);
    }
}

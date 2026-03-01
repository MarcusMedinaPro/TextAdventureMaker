// <copyright file="AiFeatureServiceBase.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Features;

public abstract class AiFeatureServiceBase(IAiProviderRouter router, AiParserOptions? options = null)
{
    protected IAiProviderRouter Router { get; } = router ?? throw new ArgumentNullException(nameof(router));
    protected AiParserOptions Options { get; } = options ?? new AiParserOptions();

    protected async Task<AiRoutingResult?> TryRouteAsync(string prompt, CancellationToken cancellationToken = default)
    {
        if (!Options.Enabled || string.IsNullOrWhiteSpace(prompt))
            return null;

        try
        {
            Probe("feature.ai.call", prompt);
            AiParseRequest request = new(
                Input: prompt,
                EstimatedTokens: Math.Max(1, Options.EstimatedTokensPerRequest));

            AiRoutingResult route = await Router.RouteAsync(request, cancellationToken).ConfigureAwait(false);
            Probe("feature.ai.response", route.CommandText ?? string.Empty);
            return route;
        }
        catch
        {
            Probe("feature.ai.error", "route_failed");
            return null;
        }
    }

    protected void Probe(string eventName, string value)
    {
        Options.DebugProbe?.Invoke(eventName, value);
    }
}

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
            AiParseRequest request = new(
                Input: prompt,
                EstimatedTokens: Math.Max(1, Options.EstimatedTokensPerRequest));

            return await Router.RouteAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }
}

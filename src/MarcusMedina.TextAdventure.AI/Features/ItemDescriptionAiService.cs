// <copyright file="ItemDescriptionAiService.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Features;

public sealed class ItemDescriptionAiService(
    IAiProviderRouter router,
    IAiDescriptionCache cache,
    AiParserOptions? options = null) : AiFeatureServiceBase(router, options), IItemDescriptionAiService
{
    private readonly IAiDescriptionCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public async Task<string?> GenerateDescriptionAsync(DescriptionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        string entityType = NormaliseEntityType(request.EntityType);
        DescriptionRequest normalisedRequest = request with { EntityType = entityType };
        string key = DescriptionCacheKeyBuilder.Build(normalisedRequest);
        if (_cache.TryGet(key, out string cached))
        {
            Probe("description.cache.hit", key);
            return cached;
        }

        string prompt = AiFeaturePrompts.BuildDescriptionPrompt(normalisedRequest);
        AiRoutingResult? route = await TryRouteAsync(prompt, cancellationToken).ConfigureAwait(false);
        string? description = route?.CommandText;
        if (string.IsNullOrWhiteSpace(description))
            return CacheFallback(normalisedRequest, key);

        string cleaned = description.Trim();
        _cache.Set(key, cleaned);
        Probe("description.cache.store", key);
        return cleaned;
    }

    private static string NormaliseEntityType(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "item";

        return raw.Trim().ToLowerInvariant();
    }

    private string? CacheFallback(DescriptionRequest request, string key)
    {
        string fallback = request.BaselinePrompt?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(fallback))
            return null;

        _cache.Set(key, fallback);
        Probe("description.cache.store", key);
        return fallback;
    }
}

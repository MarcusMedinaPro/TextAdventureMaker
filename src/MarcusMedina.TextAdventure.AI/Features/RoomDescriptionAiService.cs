// <copyright file="RoomDescriptionAiService.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Features;

public sealed class RoomDescriptionAiService(
    IAiProviderRouter router,
    IAiDescriptionCache cache,
    AiParserOptions? options = null) : AiFeatureServiceBase(router, options), IRoomDescriptionAiService
{
    private readonly IAiDescriptionCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public async Task<string?> GenerateDescriptionAsync(DescriptionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        string key = DescriptionCacheKeyBuilder.Build(request with { EntityType = "room" });
        if (_cache.TryGet(key, out string cached))
            return cached;

        string prompt = AiFeaturePrompts.BuildDescriptionPrompt(request with { EntityType = "room" });
        AiRoutingResult? route = await TryRouteAsync(prompt, cancellationToken).ConfigureAwait(false);
        string? description = route?.CommandText;
        if (string.IsNullOrWhiteSpace(description))
            return CacheFallback(request, key);

        string cleaned = description.Trim();
        _cache.Set(key, cleaned);
        return cleaned;
    }

    private string? CacheFallback(DescriptionRequest request, string key)
    {
        string fallback = request.BaselinePrompt?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(fallback))
            return null;

        _cache.Set(key, fallback);
        return fallback;
    }
}

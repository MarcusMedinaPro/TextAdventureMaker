// <copyright file="ClaudeCommandProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Providers;

public sealed class ClaudeCommandProvider : JsonHttpAiCommandProviderBase
{
    private readonly ClaudeSettings _settings;

    public ClaudeCommandProvider(ClaudeSettings settings, HttpClient? client = null)
        : base(client, settings?.TimeoutMs ?? 8000)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public override string Name => AiProviderNames.Claude;

    public override Task<AiProviderResult> ParseAsync(AiParseRequest request, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
            return Task.FromResult(AiProviderResult.Unavailable(Name, "Provider is disabled."));

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            return Task.FromResult(AiProviderResult.Unavailable(Name, "Claude API key is missing."));

        object payload = new
        {
            model = _settings.Model,
            system = request.SystemPrompt ?? _settings.SystemPrompt,
            max_tokens = _settings.MaxTokens,
            temperature = _settings.Temperature,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = request.Input
                }
            }
        };

        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase)
        {
            ["x-api-key"] = _settings.ApiKey,
            ["anthropic-version"] = "2023-06-01"
        };

        return SendJsonAsync(_settings.Endpoint, payload, headers, cancellationToken);
    }
}

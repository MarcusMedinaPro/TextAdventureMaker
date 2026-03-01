// <copyright file="GeminiCommandProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Providers;

public sealed class GeminiCommandProvider : JsonHttpAiCommandProviderBase
{
    private readonly GeminiSettings _settings;

    public GeminiCommandProvider(GeminiSettings settings, HttpClient? client = null)
        : base(client, settings?.TimeoutMs ?? 8000)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public override string Name => AiProviderNames.Gemini;

    public override Task<AiProviderResult> ParseAsync(AiParseRequest request, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
            return Task.FromResult(AiProviderResult.Unavailable(Name, "Provider is disabled."));

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            return Task.FromResult(AiProviderResult.Unavailable(Name, "Gemini API key is missing."));

        string endpoint = _settings.EndpointTemplate
            .Replace("{model}", _settings.Model, StringComparison.OrdinalIgnoreCase)
            .Replace("{apiKey}", _settings.ApiKey, StringComparison.OrdinalIgnoreCase);

        object payload = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = BuildPrompt(request, _settings.SystemPrompt) }
                    }
                }
            },
            generationConfig = new
            {
                temperature = _settings.Temperature
            }
        };

        return SendJsonAsync(endpoint, payload, cancellationToken: cancellationToken);
    }

    private static string BuildPrompt(AiParseRequest request, string? systemPrompt)
    {
        string? system = request.SystemPrompt ?? systemPrompt;
        return string.IsNullOrWhiteSpace(system)
            ? request.Input
            : $"{system}\n\nUser: {request.Input}\nAssistant:";
    }
}

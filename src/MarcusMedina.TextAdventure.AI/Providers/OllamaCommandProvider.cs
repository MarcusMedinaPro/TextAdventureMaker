// <copyright file="OllamaCommandProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Providers;

public sealed class OllamaCommandProvider : JsonHttpAiCommandProviderBase
{
    private readonly OllamaSettings _settings;

    public OllamaCommandProvider(OllamaSettings settings, HttpClient? client = null)
        : base(client, settings?.TimeoutMs ?? 5000)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public override string Name => AiProviderNames.Ollama;

    public override Task<AiProviderResult> ParseAsync(AiParseRequest request, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
            return Task.FromResult(AiProviderResult.Unavailable(Name, "Provider is disabled."));

        string prompt = BuildPrompt(request, _settings.SystemPrompt);
        string endpoint = $"{_settings.Endpoint.TrimEnd('/')}/api/generate";
        object payload = new
        {
            model = _settings.Model,
            prompt,
            stream = false,
            temperature = _settings.Temperature
        };

        return SendJsonAsync(endpoint, payload, cancellationToken: cancellationToken);
    }

    private static string BuildPrompt(AiParseRequest request, string fallbackPrompt)
    {
        string system = string.IsNullOrWhiteSpace(request.SystemPrompt) ? fallbackPrompt : request.SystemPrompt;
        return $"{system}\n\nUser: {request.Input}\nAssistant:";
    }
}

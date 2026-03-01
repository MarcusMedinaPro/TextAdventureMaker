// <copyright file="OpenAiCommandProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Providers;

public sealed class OpenAiCommandProvider : JsonHttpAiCommandProviderBase
{
    private readonly OpenAiSettings _settings;

    public OpenAiCommandProvider(OpenAiSettings settings, HttpClient? client = null)
        : base(client, settings?.TimeoutMs ?? 8000)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public override string Name => AiProviderNames.OpenAi;

    public override Task<AiProviderResult> ParseAsync(AiParseRequest request, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
            return Task.FromResult(AiProviderResult.Unavailable(Name, "Provider is disabled."));

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            return Task.FromResult(AiProviderResult.Unavailable(Name, "OpenAI API key is missing."));

        object payload = new
        {
            model = _settings.Model,
            input = BuildPrompt(request, _settings.SystemPrompt),
            temperature = _settings.Temperature
        };

        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Authorization"] = $"Bearer {_settings.ApiKey}"
        };

        return SendJsonAsync(_settings.Endpoint, payload, headers, cancellationToken);
    }

    private static string BuildPrompt(AiParseRequest request, string? systemPrompt)
    {
        string? system = string.IsNullOrWhiteSpace(request.SystemPrompt) ? systemPrompt : request.SystemPrompt;
        if (string.IsNullOrWhiteSpace(system))
            return request.Input;

        return $"{system}\n\nUser: {request.Input}\nAssistant:";
    }
}

// <copyright file="OneMinAiCommandProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Providers;

public sealed class OneMinAiCommandProvider : JsonHttpAiCommandProviderBase
{
    private readonly OneMinAiSettings _settings;

    public OneMinAiCommandProvider(OneMinAiSettings settings, HttpClient? client = null)
        : base(client, settings?.TimeoutMs ?? 8000)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public override string Name => AiProviderNames.OneMinAi;

    public override Task<AiProviderResult> ParseAsync(AiParseRequest request, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
            return Task.FromResult(AiProviderResult.Unavailable(Name, "Provider is disabled."));

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            return Task.FromResult(AiProviderResult.Unavailable(Name, "1minAI API key is missing."));

        OneMinAiPayload payload = new(
            Feature: _settings.Feature ?? "ai-text-chat",
            Type: "text",
            Model: _settings.Model,
            Prompt: BuildPrompt(request, _settings.SystemPrompt),
            Temperature: _settings.Temperature);

        Dictionary<string, string> headers = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Authorization"] = $"Bearer {_settings.ApiKey}",
            ["API-KEY"] = _settings.ApiKey
        };

        return SendJsonAsync(_settings.Endpoint, payload, headers, cancellationToken);
    }

    private static string BuildPrompt(AiParseRequest request, string? systemPrompt)
    {
        string? system = request.SystemPrompt ?? systemPrompt;
        return string.IsNullOrWhiteSpace(system)
            ? request.Input
            : $"{system}\n\nUser: {request.Input}\nAssistant:";
    }

    private sealed record OneMinAiPayload(
        string Feature,
        string Type,
        string Model,
        string Prompt,
        double Temperature);
}

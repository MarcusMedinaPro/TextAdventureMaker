// <copyright file="LmStudioCommandProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Providers;

public sealed class LmStudioCommandProvider : JsonHttpAiCommandProviderBase
{
    private readonly LmStudioSettings _settings;

    public LmStudioCommandProvider(LmStudioSettings settings, HttpClient? client = null)
        : base(client, settings?.TimeoutMs ?? 8000)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public override string Name => AiProviderNames.LmStudio;

    public override Task<AiProviderResult> ParseAsync(AiParseRequest request, CancellationToken cancellationToken = default)
    {
        if (!_settings.Enabled)
            return Task.FromResult(AiProviderResult.Unavailable(Name, "Provider is disabled."));

        object payload = new
        {
            model = _settings.Model,
            temperature = _settings.Temperature,
            messages = BuildMessages(request, _settings.SystemPrompt)
        };

        Dictionary<string, string>? headers = null;
        if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Authorization"] = $"Bearer {_settings.ApiKey}"
            };
        }

        return SendJsonAsync(_settings.Endpoint, payload, headers, cancellationToken);
    }

    private static object[] BuildMessages(AiParseRequest request, string? systemPrompt)
    {
        string? system = request.SystemPrompt ?? systemPrompt;

        return string.IsNullOrWhiteSpace(system)
            ? [new { role = "user", content = request.Input }]
            : [new { role = "system", content = system }, new { role = "user", content = request.Input }];
    }
}

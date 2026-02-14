// <copyright file="OllamaCommandParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text;
using System.Text.Json;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI;

public sealed class OllamaCommandParser : ICommandParser
{
    private readonly ICommandParser _fallback;
    private readonly HttpClient _client;

    public OllamaSettings Settings { get; }

    public OllamaCommandParser(OllamaSettings settings, ICommandParser fallback)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
        _client = new HttpClient { Timeout = TimeSpan.FromMilliseconds(settings.TimeoutMs) };
    }

    public ICommand Parse(string input)
    {
        if (!Settings.Enabled)
            return _fallback.Parse(input);

        try
        {
            var ollamsResponse = GetOllamaResponse(input);
            if (ollamsResponse != null)
            {
                var command = _fallback.Parse(ollamsResponse);
                if (command != null)
                    return command;
            }
        }
        catch
        {
            // Silently fall back to keyword parser on any error
        }

        return _fallback.Parse(input);
    }

    private string? GetOllamaResponse(string input)
    {
        try
        {
            var endpoint = $"{Settings.Endpoint.TrimEnd('/')}/api/generate";
            var requestBody = new
            {
                model = Settings.Model,
                prompt = $"{Settings.SystemPrompt}\n\nUser: {input}\nAssistant:",
                stream = false,
                temperature = Settings.Temperature
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(Settings.TimeoutMs));
            var response = _client.PostAsync(endpoint, content, cts.Token).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
                return null;

            var responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            var jsonDoc = JsonDocument.Parse(responseText);
            var root = jsonDoc.RootElement;

            if (root.TryGetProperty("response", out var responseProperty))
            {
                return responseProperty.GetString()?.Trim();
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}

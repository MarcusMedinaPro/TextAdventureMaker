// <copyright file="OllamaCommandProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Providers;

using System.Text;
using System.Text.Json;

public sealed class OllamaCommandProvider : JsonHttpAiCommandProviderBase
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

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
        OllamaGenerateRequest payload = new(
            Model: _settings.Model,
            Prompt: prompt,
            Stream: false,
            Temperature: _settings.Temperature);

        return SendGenerateRequestAsync(endpoint, payload, cancellationToken);
    }

    private static string BuildPrompt(AiParseRequest request, string fallbackPrompt)
    {
        string system = string.IsNullOrWhiteSpace(request.SystemPrompt) ? fallbackPrompt : request.SystemPrompt;
        return $"{system}\n\nUser: {request.Input}\nAssistant:";
    }

    private async Task<AiProviderResult> SendGenerateRequestAsync(
        string endpoint,
        OllamaGenerateRequest payload,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
            return AiProviderResult.Unavailable(Name, "Provider endpoint is not configured.");

        using HttpRequestMessage message = new(HttpMethod.Post, endpoint);
        string json = JsonSerializer.Serialize(payload, SerializerOptions);
        message.Content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using HttpResponseMessage response = await Client.SendAsync(message, cancellationToken).ConfigureAwait(false);
            string responseText = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                return AiProviderResult.Failed(Name, $"{(int)response.StatusCode} {response.ReasonPhrase}".Trim());

            OllamaGenerateResponse? parsed = JsonSerializer.Deserialize<OllamaGenerateResponse>(responseText, SerializerOptions);
            string? command = parsed?.Response?.Trim();
            if (string.IsNullOrWhiteSpace(command))
                return AiProviderResult.InvalidOutput(Name, "Provider returned no valid command.");

            AiTokenUsage? usage = BuildTokenUsage(parsed);
            return AiProviderResult.Success(Name, command, usage);
        }
        catch (OperationCanceledException)
        {
            return AiProviderResult.Failed(Name, "Request timed out.");
        }
        catch (Exception ex)
        {
            return AiProviderResult.Failed(Name, ex.Message);
        }
    }

    private static AiTokenUsage? BuildTokenUsage(OllamaGenerateResponse? response)
    {
        if (response == null)
            return null;

        int input = Math.Max(0, response.PromptEvalCount ?? 0);
        int output = Math.Max(0, response.EvalCount ?? 0);
        if (input == 0 && output == 0)
            return null;

        return new AiTokenUsage(input, output);
    }

    private sealed record OllamaGenerateRequest(
        string Model,
        string Prompt,
        bool Stream,
        double Temperature);

    private sealed record OllamaGenerateResponse(
        string? Response,
        int? PromptEvalCount,
        int? EvalCount);
}

// <copyright file="JsonHttpAiCommandProviderBase.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text;
using System.Text.Json;

namespace MarcusMedina.TextAdventure.AI.Providers;

public abstract class JsonHttpAiCommandProviderBase : IAiCommandProvider, IDisposable
{
    private readonly bool _ownsClient;
    protected readonly HttpClient Client;

    protected JsonHttpAiCommandProviderBase(HttpClient? client, int timeoutMs)
    {
        _ownsClient = client is null;
        Client = client ?? new HttpClient();
        Client.Timeout = TimeSpan.FromMilliseconds(Math.Max(250, timeoutMs));
    }

    public abstract string Name { get; }
    public abstract Task<AiProviderResult> ParseAsync(AiParseRequest request, CancellationToken cancellationToken = default);

    protected async Task<AiProviderResult> SendJsonAsync(
        string endpoint,
        object payload,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
            return AiProviderResult.Unavailable(Name, "Provider endpoint is not configured.");

        using HttpRequestMessage message = new(HttpMethod.Post, endpoint);

        if (headers != null)
        {
            foreach ((string key, string value) in headers)
            {
                if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                    _ = message.Headers.TryAddWithoutValidation(key, value);
            }
        }

        string json = JsonSerializer.Serialize(payload);
        message.Content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using HttpResponseMessage response = await Client.SendAsync(message, cancellationToken).ConfigureAwait(false);
            string responseText = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                return AiProviderResult.Failed(Name, $"{(int)response.StatusCode} {response.ReasonPhrase}".Trim());

            using JsonDocument document = JsonDocument.Parse(responseText);
            string? commandText = AiOutputNormalizer.ExtractCommandText(document.RootElement);
            AiTokenUsage? tokenUsage = AiOutputNormalizer.ExtractTokenUsage(document.RootElement);

            return string.IsNullOrWhiteSpace(commandText)
                ? AiProviderResult.InvalidOutput(Name, "Provider returned no valid command.")
                : AiProviderResult.Success(Name, commandText, tokenUsage);
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

    public void Dispose()
    {
        if (_ownsClient)
            Client.Dispose();
    }
}

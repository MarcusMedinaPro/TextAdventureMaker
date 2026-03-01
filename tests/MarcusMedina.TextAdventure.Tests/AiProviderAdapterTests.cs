// <copyright file="AiProviderAdapterTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using System.Net;
using System.Net.Http;
using System.Text;
using MarcusMedina.TextAdventure.AI.Models;
using MarcusMedina.TextAdventure.AI.Providers;
using MarcusMedina.TextAdventure.AI.Settings;

public class AiProviderAdapterTests
{
    [Fact]
    public async Task OpenAiCommandProvider_ParseAsync_MapsCommandAndUsage()
    {
        string? authHeader = null;
        StubHttpMessageHandler handler = new((request, _) =>
        {
            request.Headers.TryGetValues("Authorization", out IEnumerable<string>? values);
            authHeader = values?.FirstOrDefault();
            return JsonResponse(HttpStatusCode.OK, """{"output":[{"content":[{"text":"go north"}]}],"usage":{"input_tokens":2,"output_tokens":1}}""");
        });

        using HttpClient client = new(handler);
        OpenAiCommandProvider provider = new(new OpenAiSettings("secret-key"), client);

        AiProviderResult result = await provider.ParseAsync(new AiParseRequest("inspect room"), TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal("go north", result.CommandText);
        Assert.Equal(3, result.TokenUsage?.TotalTokens);
        Assert.Equal("Bearer secret-key", authHeader);
    }

    [Fact]
    public async Task OpenAiCommandProvider_MissingApiKey_ReturnsUnavailable()
    {
        OpenAiCommandProvider provider = new(new OpenAiSettings(apiKey: ""));

        AiProviderResult result = await provider.ParseAsync(new AiParseRequest("look"), TestContext.Current.CancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Equal(AiAttemptOutcome.SkippedUnavailable, result.Outcome);
        Assert.Contains("missing", result.Message ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task OpenAiCommandProvider_EmptyJsonBody_ReturnsInvalidOutput()
    {
        StubHttpMessageHandler handler = new((_, _) => JsonResponse(HttpStatusCode.OK, "{}"));
        using HttpClient client = new(handler);
        OpenAiCommandProvider provider = new(new OpenAiSettings("secret-key"), client);

        AiProviderResult result = await provider.ParseAsync(new AiParseRequest("look"), TestContext.Current.CancellationToken);

        Assert.Equal(AiAttemptOutcome.InvalidOutput, result.Outcome);
    }

    [Fact]
    public async Task ClaudeCommandProvider_ParseAsync_MapsTextField()
    {
        string? apiKeyHeader = null;
        StubHttpMessageHandler handler = new((request, _) =>
        {
            request.Headers.TryGetValues("x-api-key", out IEnumerable<string>? values);
            apiKeyHeader = values?.FirstOrDefault();
            return JsonResponse(HttpStatusCode.OK, """{"text":"go north"}""");
        });

        using HttpClient client = new(handler);
        ClaudeCommandProvider provider = new(new ClaudeSettings("claude-key"), client);

        AiProviderResult result = await provider.ParseAsync(new AiParseRequest("move"), TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal("go north", result.CommandText);
        Assert.Equal("claude-key", apiKeyHeader);
    }

    [Fact]
    public async Task MistralCommandProvider_ParseAsync_MapsChoiceContent()
    {
        StubHttpMessageHandler handler = new((_, _) =>
            JsonResponse(HttpStatusCode.OK, """{"choices":[{"message":{"content":"take key"}}],"usage":{"prompt_tokens":1,"completion_tokens":2}}"""));

        using HttpClient client = new(handler);
        MistralCommandProvider provider = new(new MistralSettings("mistral-key"), client);

        AiProviderResult result = await provider.ParseAsync(new AiParseRequest("grab key"), TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal("take key", result.CommandText);
        Assert.Equal(3, result.TokenUsage?.TotalTokens);
    }

    [Fact]
    public async Task OpenRouterCommandProvider_ParseAsync_MapsChoiceContent()
    {
        StubHttpMessageHandler handler = new((_, _) =>
            JsonResponse(HttpStatusCode.OK, """{"choices":[{"message":{"content":"inventory"}}]}"""));

        using HttpClient client = new(handler);
        OpenRouterCommandProvider provider = new(new OpenRouterSettings("router-key"), client);

        AiProviderResult result = await provider.ParseAsync(new AiParseRequest("what am i carrying"), TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal("inventory", result.CommandText);
    }

    [Fact]
    public async Task OneMinAiCommandProvider_ParseAsync_MapsNestedDataTextAndSendsApiKeyHeader()
    {
        string? apiKeyHeader = null;
        string? requestBody = null;
        StubHttpMessageHandler handler = new((request, cancellationToken) =>
        {
            _ = request.Headers.TryGetValues("API-KEY", out IEnumerable<string>? values);
            apiKeyHeader = values?.FirstOrDefault();
            requestBody = request.Content?.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();
            return JsonResponse(HttpStatusCode.OK, """{"data":{"text":"take key"}}""");
        });

        using HttpClient client = new(handler);
        OneMinAiCommandProvider provider = new(new OneMinAiSettings("one-key"), client);

        AiProviderResult result = await provider.ParseAsync(new AiParseRequest("inspect"), TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal("take key", result.CommandText);
        Assert.Equal("one-key", apiKeyHeader);
        Assert.Contains("\"feature\":\"ai-text-chat\"", requestBody ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GeminiCommandProvider_ParseAsync_MapsCandidateTextAndUsage()
    {
        StubHttpMessageHandler handler = new((_, _) =>
            JsonResponse(HttpStatusCode.OK, """{"candidates":[{"content":{"parts":[{"text":"talk guard"}]}}],"usage":{"promptTokenCount":3,"candidatesTokenCount":2}}"""));

        using HttpClient client = new(handler);
        GeminiCommandProvider provider = new(new GeminiSettings("gem-key"), client);

        AiProviderResult result = await provider.ParseAsync(new AiParseRequest("speak to guard"), TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal("talk guard", result.CommandText);
        Assert.Equal(5, result.TokenUsage?.TotalTokens);
    }

    [Fact]
    public async Task OllamaCommandProvider_ParseAsync_MapsResponseField()
    {
        StubHttpMessageHandler handler = new((_, _) => JsonResponse(HttpStatusCode.OK, """{"response":"go north"}"""));

        using HttpClient client = new(handler);
        OllamaCommandProvider provider = new(new OllamaSettings("http://localhost:11434"), client);

        AiProviderResult result = await provider.ParseAsync(new AiParseRequest("inspect room"), TestContext.Current.CancellationToken);

        Assert.True(result.IsSuccess);
        Assert.Equal("go north", result.CommandText);
    }

    [Fact]
    public async Task Provider_NonSuccessStatus_ReturnsFailed()
    {
        StubHttpMessageHandler handler = new((_, _) => JsonResponse(HttpStatusCode.BadGateway, """{"error":"upstream failed"}"""));
        using HttpClient client = new(handler);
        MistralCommandProvider provider = new(new MistralSettings("mistral-key"), client);

        AiProviderResult result = await provider.ParseAsync(new AiParseRequest("look"), TestContext.Current.CancellationToken);

        Assert.Equal(AiAttemptOutcome.Failed, result.Outcome);
        Assert.Contains("502", result.Message ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, string json)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handler) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = handler(request, cancellationToken);
            return Task.FromResult(response);
        }
    }
}

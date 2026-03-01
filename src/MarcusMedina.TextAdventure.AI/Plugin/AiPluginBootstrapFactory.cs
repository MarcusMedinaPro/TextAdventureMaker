// <copyright file="AiPluginBootstrapFactory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.AI.Plugin;

public static class AiPluginBootstrapFactory
{
    public static AiPluginBootstrap Create(AiPluginInitOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        ICommandParser baseParser = options.BaseParser ?? new KeywordParser(KeywordParserConfig.Default);
        AiCommandParserBuilder builder = new AiCommandParserBuilder()
            .WithFallback(baseParser)
            .WithEnabled(options.ParserOptions.Enabled)
            .WithStrictMode(options.ParserOptions.StrictMode)
            .WithPreferLocalCommandFirst(options.ParserOptions.PreferLocalCommandFirst)
            .WithFallbackOnSafetyRejection(options.ParserOptions.FallbackOnSafetyRejection)
            .WithFallbackOnInvalidAiCommand(options.ParserOptions.FallbackOnInvalidAiCommand)
            .WithTimeoutMs(options.ParserOptions.TimeoutMs)
            .WithEstimatedTokensPerRequest(options.ParserOptions.EstimatedTokensPerRequest)
            .WithDebugProbe(options.ParserOptions.DebugProbe)
            .WithTelemetrySink(options.ParserOptions.TelemetrySink)
            .WithRouterDecorator(options.RouterDecorator);

        AddProvider(builder, options.PrimaryProvider);
        foreach (AiProviderInitOptions provider in options.FallbackProviders)
            AddProvider(builder, provider);

        AiCommandParser aiParser = builder.Build();
        AiFeatureModule module = builder.BuildFeatureModule(options.DescriptionCache);
        ICommandParser parserWithPlugin = aiParser.WithAiPlugin(module, options.PluginOptions);

        return new AiPluginBootstrap
        {
            Parser = parserWithPlugin,
            AiParser = aiParser,
            Module = module,
            PluginOptions = options.PluginOptions
        };
    }

    private static void AddProvider(AiCommandParserBuilder builder, AiProviderInitOptions init)
    {
        switch (init.Provider)
        {
            case AiProviderKind.Ollama:
                builder.UseOllama(CreateOllamaSettings(init));
                break;
            case AiProviderKind.LmStudio:
                builder.UseLmStudio(CreateLmStudioSettings(init));
                break;
            case AiProviderKind.DockerAi:
                builder.UseDockerAi(CreateDockerAiSettings(init));
                break;
            case AiProviderKind.OpenAi:
                builder.UseOpenAi(CreateOpenAiSettings(init));
                break;
            case AiProviderKind.Claude:
                builder.UseClaude(CreateClaudeSettings(init));
                break;
            case AiProviderKind.Mistral:
                builder.UseMistral(CreateMistralSettings(init));
                break;
            case AiProviderKind.OpenRouter:
                builder.UseOpenRouter(CreateOpenRouterSettings(init));
                break;
            case AiProviderKind.OneMinAi:
                builder.UseOneMinAi(CreateOneMinAiSettings(init));
                break;
            case AiProviderKind.Gemini:
                builder.UseGemini(CreateGeminiSettings(init));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(init.Provider), init.Provider, "Unsupported AI provider.");
        }
    }

    private static OllamaSettings CreateOllamaSettings(AiProviderInitOptions init)
    {
        OllamaSettings settings = new()
        {
            Enabled = init.Enabled
        };

        if (!string.IsNullOrWhiteSpace(init.Endpoint))
            settings.Endpoint = init.Endpoint;
        if (!string.IsNullOrWhiteSpace(init.Model))
            settings.Model = init.Model;
        if (!string.IsNullOrWhiteSpace(init.SystemPrompt))
            settings.SystemPrompt = init.SystemPrompt;
        if (init.TimeoutMs is int timeoutMs)
            settings.TimeoutMs = timeoutMs;
        if (init.Temperature is double temperature)
            settings.Temperature = temperature;

        return settings;
    }

    private static OpenAiSettings CreateOpenAiSettings(AiProviderInitOptions init)
    {
        EnsureApiKey(init);
        OpenAiSettings settings = new(init.ApiKey!)
        {
            Enabled = init.Enabled
        };

        if (!string.IsNullOrWhiteSpace(init.Endpoint))
            settings.Endpoint = init.Endpoint;
        if (!string.IsNullOrWhiteSpace(init.Model))
            settings.Model = init.Model;
        if (!string.IsNullOrWhiteSpace(init.SystemPrompt))
            settings.SystemPrompt = init.SystemPrompt;
        if (init.TimeoutMs is int timeoutMs)
            settings.TimeoutMs = timeoutMs;
        if (init.Temperature is double temperature)
            settings.Temperature = temperature;

        return settings;
    }

    private static LmStudioSettings CreateLmStudioSettings(AiProviderInitOptions init)
    {
        LmStudioSettings settings = new()
        {
            Enabled = init.Enabled,
            ApiKey = init.ApiKey
        };

        if (!string.IsNullOrWhiteSpace(init.Endpoint))
            settings.Endpoint = init.Endpoint;
        if (!string.IsNullOrWhiteSpace(init.Model))
            settings.Model = init.Model;
        if (!string.IsNullOrWhiteSpace(init.SystemPrompt))
            settings.SystemPrompt = init.SystemPrompt;
        if (init.TimeoutMs is int timeoutMs)
            settings.TimeoutMs = timeoutMs;
        if (init.Temperature is double temperature)
            settings.Temperature = temperature;

        return settings;
    }

    private static DockerAiSettings CreateDockerAiSettings(AiProviderInitOptions init)
    {
        DockerAiSettings settings = new()
        {
            Enabled = init.Enabled,
            ApiKey = init.ApiKey
        };

        if (!string.IsNullOrWhiteSpace(init.Endpoint))
            settings.Endpoint = init.Endpoint;
        if (!string.IsNullOrWhiteSpace(init.Model))
            settings.Model = init.Model;
        if (!string.IsNullOrWhiteSpace(init.SystemPrompt))
            settings.SystemPrompt = init.SystemPrompt;
        if (init.TimeoutMs is int timeoutMs)
            settings.TimeoutMs = timeoutMs;
        if (init.Temperature is double temperature)
            settings.Temperature = temperature;

        return settings;
    }

    private static ClaudeSettings CreateClaudeSettings(AiProviderInitOptions init)
    {
        EnsureApiKey(init);
        ClaudeSettings settings = new(init.ApiKey!)
        {
            Enabled = init.Enabled
        };

        if (!string.IsNullOrWhiteSpace(init.Endpoint))
            settings.Endpoint = init.Endpoint;
        if (!string.IsNullOrWhiteSpace(init.Model))
            settings.Model = init.Model;
        if (!string.IsNullOrWhiteSpace(init.SystemPrompt))
            settings.SystemPrompt = init.SystemPrompt;
        if (init.TimeoutMs is int timeoutMs)
            settings.TimeoutMs = timeoutMs;
        if (init.Temperature is double temperature)
            settings.Temperature = temperature;

        return settings;
    }

    private static MistralSettings CreateMistralSettings(AiProviderInitOptions init)
    {
        EnsureApiKey(init);
        MistralSettings settings = new(init.ApiKey!)
        {
            Enabled = init.Enabled
        };

        if (!string.IsNullOrWhiteSpace(init.Endpoint))
            settings.Endpoint = init.Endpoint;
        if (!string.IsNullOrWhiteSpace(init.Model))
            settings.Model = init.Model;
        if (!string.IsNullOrWhiteSpace(init.SystemPrompt))
            settings.SystemPrompt = init.SystemPrompt;
        if (init.TimeoutMs is int timeoutMs)
            settings.TimeoutMs = timeoutMs;
        if (init.Temperature is double temperature)
            settings.Temperature = temperature;

        return settings;
    }

    private static OpenRouterSettings CreateOpenRouterSettings(AiProviderInitOptions init)
    {
        EnsureApiKey(init);
        OpenRouterSettings settings = new(init.ApiKey!)
        {
            Enabled = init.Enabled
        };

        if (!string.IsNullOrWhiteSpace(init.Endpoint))
            settings.Endpoint = init.Endpoint;
        if (!string.IsNullOrWhiteSpace(init.Model))
            settings.Model = init.Model;
        if (!string.IsNullOrWhiteSpace(init.SystemPrompt))
            settings.SystemPrompt = init.SystemPrompt;
        if (init.TimeoutMs is int timeoutMs)
            settings.TimeoutMs = timeoutMs;
        if (init.Temperature is double temperature)
            settings.Temperature = temperature;

        return settings;
    }

    private static OneMinAiSettings CreateOneMinAiSettings(AiProviderInitOptions init)
    {
        EnsureApiKey(init);
        OneMinAiSettings settings = new(init.ApiKey!)
        {
            Enabled = init.Enabled
        };

        if (!string.IsNullOrWhiteSpace(init.Endpoint))
            settings.Endpoint = init.Endpoint;
        if (!string.IsNullOrWhiteSpace(init.Model))
            settings.Model = init.Model;
        if (!string.IsNullOrWhiteSpace(init.SystemPrompt))
            settings.SystemPrompt = init.SystemPrompt;
        if (init.TimeoutMs is int timeoutMs)
            settings.TimeoutMs = timeoutMs;
        if (init.Temperature is double temperature)
            settings.Temperature = temperature;
        if (init.DailyTokenLimit is int dailyLimit)
            settings.DailyTokenLimit = Math.Max(0, dailyLimit);

        return settings;
    }

    private static GeminiSettings CreateGeminiSettings(AiProviderInitOptions init)
    {
        EnsureApiKey(init);
        GeminiSettings settings = new(init.ApiKey!)
        {
            Enabled = init.Enabled
        };

        if (!string.IsNullOrWhiteSpace(init.Endpoint))
            settings.EndpointTemplate = init.Endpoint;
        if (!string.IsNullOrWhiteSpace(init.Model))
            settings.Model = init.Model;
        if (!string.IsNullOrWhiteSpace(init.SystemPrompt))
            settings.SystemPrompt = init.SystemPrompt;
        if (init.TimeoutMs is int timeoutMs)
            settings.TimeoutMs = timeoutMs;
        if (init.Temperature is double temperature)
            settings.Temperature = temperature;

        return settings;
    }

    private static void EnsureApiKey(AiProviderInitOptions init)
    {
        if (!string.IsNullOrWhiteSpace(init.ApiKey))
            return;

        throw new ArgumentException($"Provider '{init.Provider}' requires an API key.", nameof(init));
    }
}

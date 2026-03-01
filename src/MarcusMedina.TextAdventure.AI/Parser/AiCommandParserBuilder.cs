// <copyright file="AiCommandParserBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.AI;

public class AiCommandParserBuilder
{
    private readonly OllamaSettings _settings = new();
    private readonly AiParserOptions _options = new();
    private readonly List<IAiCommandProvider> _providers = [];
    private ICommandParser? _fallback;
    private IAiCommandSafetyPolicy? _safetyPolicy;
    private ITokenBudgetPolicy? _tokenBudgetPolicy;
    private Func<IAiProviderRouter, IAiProviderRouter>? _routerDecorator;

    public AiCommandParserBuilder WithEndpoint(string endpoint)
    {
        _settings.Endpoint = endpoint ?? "";
        return this;
    }

    public AiCommandParserBuilder WithModel(string model)
    {
        _settings.Model = model ?? "";
        return this;
    }

    public AiCommandParserBuilder WithSystemPrompt(string prompt)
    {
        _settings.SystemPrompt = prompt ?? "";
        return this;
    }

    public AiCommandParserBuilder WithFallback(ICommandParser parser)
    {
        _fallback = parser;
        return this;
    }

    public AiCommandParserBuilder WithStrictMode(bool strict = true)
    {
        _options.StrictMode = strict;
        return this;
    }

    public AiCommandParserBuilder WithEnabled(bool enabled = true)
    {
        _options.Enabled = enabled;
        _settings.Enabled = enabled;
        return this;
    }

    public AiCommandParserBuilder WithPreferLocalCommandFirst(bool preferLocal = true)
    {
        _options.PreferLocalCommandFirst = preferLocal;
        return this;
    }

    public AiCommandParserBuilder WithFallbackOnSafetyRejection(bool fallback = true)
    {
        _options.FallbackOnSafetyRejection = fallback;
        return this;
    }

    public AiCommandParserBuilder WithFallbackOnInvalidAiCommand(bool fallback = true)
    {
        _options.FallbackOnInvalidAiCommand = fallback;
        return this;
    }

    public AiCommandParserBuilder WithDebugProbe(Action<string, string>? probe)
    {
        _options.DebugProbe = probe;
        return this;
    }

    public AiCommandParserBuilder WithTimeoutMs(int timeoutMs)
    {
        _options.TimeoutMs = timeoutMs;
        _settings.TimeoutMs = timeoutMs;
        return this;
    }

    public AiCommandParserBuilder WithEstimatedTokensPerRequest(int tokens)
    {
        _options.EstimatedTokensPerRequest = Math.Max(1, tokens);
        return this;
    }

    public AiCommandParserBuilder WithSafetyPolicy(IAiCommandSafetyPolicy policy)
    {
        _safetyPolicy = policy;
        return this;
    }

    public AiCommandParserBuilder WithTokenBudgetPolicy(ITokenBudgetPolicy policy)
    {
        _tokenBudgetPolicy = policy;
        return this;
    }

    public AiCommandParserBuilder WithRouterDecorator(Func<IAiProviderRouter, IAiProviderRouter>? decorator)
    {
        _routerDecorator = decorator;
        return this;
    }

    public AiCommandParserBuilder UseProvider(IAiCommandProvider provider)
    {
        _providers.Add(provider);
        return this;
    }

    public AiCommandParserBuilder UseOllama(OllamaSettings? settings = null)
    {
        OllamaSettings source = settings ?? _settings;
        _providers.Add(new OllamaCommandProvider(source));
        return this;
    }

    public AiCommandParserBuilder UseOpenAi(OpenAiSettings settings)
    {
        _providers.Add(new OpenAiCommandProvider(settings));
        return this;
    }

    public AiCommandParserBuilder UseLmStudio(LmStudioSettings settings)
    {
        _providers.Add(new LmStudioCommandProvider(settings));
        return this;
    }

    public AiCommandParserBuilder UseDockerAi(DockerAiSettings settings)
    {
        _providers.Add(new DockerAiCommandProvider(settings));
        return this;
    }

    public AiCommandParserBuilder UseClaude(ClaudeSettings settings)
    {
        _providers.Add(new ClaudeCommandProvider(settings));
        return this;
    }

    public AiCommandParserBuilder UseMistral(MistralSettings settings)
    {
        _providers.Add(new MistralCommandProvider(settings));
        return this;
    }

    public AiCommandParserBuilder UseOpenRouter(OpenRouterSettings settings)
    {
        _providers.Add(new OpenRouterCommandProvider(settings));
        return this;
    }

    public AiCommandParserBuilder UseOneMinAi(OneMinAiSettings settings)
    {
        _providers.Add(new OneMinAiCommandProvider(settings));

        _tokenBudgetPolicy ??= new InMemoryDailyTokenBudgetPolicy();
        if (_tokenBudgetPolicy is InMemoryDailyTokenBudgetPolicy inMemoryBudget)
            _ = inMemoryBudget.SetDailyLimit(AiProviderNames.OneMinAi, Math.Max(0, settings.DailyTokenLimit));

        return this;
    }

    public AiCommandParserBuilder UseGemini(GeminiSettings settings)
    {
        _providers.Add(new GeminiCommandProvider(settings));
        return this;
    }

    public AiCommandParser Build()
    {
        ICommandParser fallback = _fallback ?? new KeywordParser(KeywordParserConfig.Default);
        IAiProviderRouter router = BuildRouter();

        return new AiCommandParser(
            router,
            fallback,
            _safetyPolicy ?? new CommandAllowlistSafetyPolicy(),
            _options,
            _settings.SystemPrompt);
    }

    public IAiProviderRouter BuildRouter()
    {
        IAiCommandProvider[] providers = _providers.Count == 0
            ? [new OllamaCommandProvider(_settings)]
            : [.. _providers];

        _options.Enabled = _settings.Enabled;
        IAiProviderRouter router = new AiProviderRouter(providers, _tokenBudgetPolicy);
        IAiProviderRouter? decorated = _routerDecorator?.Invoke(router);
        return decorated ?? router;
    }

    public AiFeatureModule BuildFeatureModule(IAiDescriptionCache? cache = null)
    {
        return AiFeatureModule.Create(BuildRouter(), cache, _options);
    }
}

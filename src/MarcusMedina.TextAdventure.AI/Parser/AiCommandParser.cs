// <copyright file="AiCommandParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI;

public class AiCommandParser : ICommandParser
{
    private readonly IAiProviderRouter _router;
    private readonly ICommandParser _fallback;
    private readonly IAiCommandSafetyPolicy _safetyPolicy;
    private readonly AiParserOptions _options;
    private readonly string? _systemPrompt;

    public OllamaSettings Settings { get; }
    public AiRoutingResult? LastRoutingResult { get; private set; }

    public AiCommandParser(OllamaSettings settings, ICommandParser fallback)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
        _safetyPolicy = new CommandAllowlistSafetyPolicy();
        _options = new AiParserOptions
        {
            Enabled = settings.Enabled,
            TimeoutMs = settings.TimeoutMs
        };
        _systemPrompt = settings.SystemPrompt;
        _router = new AiProviderRouter([new OllamaCommandProvider(settings)]);
    }

    public AiCommandParser(
        IAiProviderRouter router,
        ICommandParser fallback,
        IAiCommandSafetyPolicy? safetyPolicy = null,
        AiParserOptions? options = null,
        string? systemPrompt = null)
    {
        _router = router ?? throw new ArgumentNullException(nameof(router));
        _fallback = fallback ?? throw new ArgumentNullException(nameof(fallback));
        _safetyPolicy = safetyPolicy ?? new CommandAllowlistSafetyPolicy();
        _options = options ?? new AiParserOptions();
        _systemPrompt = systemPrompt;
        Settings = new OllamaSettings { Enabled = _options.Enabled, TimeoutMs = _options.TimeoutMs };
    }

    public ICommand Parse(string input)
    {
        if (!_options.Enabled || string.IsNullOrWhiteSpace(input))
            return _fallback.Parse(input);

        ICommand? fallbackCommand = null;
        if (_options.PreferLocalCommandFirst && !_options.StrictMode)
        {
            fallbackCommand = _fallback.Parse(input);
            if (IsAcceptableCommand(fallbackCommand))
                return fallbackCommand;
        }

        AiRoutingResult? routingResult = null;

        try
        {
            using CancellationTokenSource cts = new(TimeSpan.FromMilliseconds(Math.Max(250, _options.TimeoutMs)));
            AiParseRequest request = new(
                Input: input,
                SystemPrompt: _systemPrompt,
                EstimatedTokens: _options.EstimatedTokensPerRequest);

            routingResult = _router.RouteAsync(request, cts.Token).GetAwaiter().GetResult();
            LastRoutingResult = routingResult;

            if (!routingResult.HasCommand || string.IsNullOrWhiteSpace(routingResult.CommandText))
                return _fallback.Parse(input);

            AiSafetyDecision safety = _safetyPolicy.Evaluate(routingResult.CommandText);
            if (!safety.IsAllowed)
            {
                if (_options.StrictMode || !_options.FallbackOnSafetyRejection)
                    return new ParserErrorCommand(safety.Message ?? "AI command rejected by safety policy.");

                return _fallback.Parse(input);
            }

            ICommand aiCommand = _fallback.Parse(routingResult.CommandText);
            if (IsAcceptableCommand(aiCommand))
                return aiCommand;

            if (_options.StrictMode || !_options.FallbackOnInvalidAiCommand)
                return new ParserErrorCommand("AI output could not be parsed into a valid command.");
        }
        catch
        {
            if (_options.StrictMode)
                return new ParserErrorCommand("AI parser failed unexpectedly.");
        }

        return fallbackCommand ?? _fallback.Parse(input);
    }

    private static bool IsAcceptableCommand(ICommand command)
    {
        return command is not UnknownCommand and not ParserErrorCommand;
    }

    public bool TryGetLastProviderName(out string? providerName)
    {
        providerName = LastRoutingResult?.ProviderName;
        return !string.IsNullOrWhiteSpace(providerName);
    }
}

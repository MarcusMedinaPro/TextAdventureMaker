// <copyright file="AiParserPipelineTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.AI;
using MarcusMedina.TextAdventure.AI.Contracts;
using MarcusMedina.TextAdventure.AI.Models;
using MarcusMedina.TextAdventure.AI.Policies;
using MarcusMedina.TextAdventure.AI.Router;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.Tests;

public class AiParserPipelineTests
{
    [Fact]
    public async Task Router_ReturnsFirstSuccessfulProvider()
    {
        IAiCommandProvider first = new StubProvider(AiProviderNames.Ollama, AiProviderResult.InvalidOutput(AiProviderNames.Ollama, "No command."));
        IAiCommandProvider second = new StubProvider(AiProviderNames.OpenAi, AiProviderResult.Success(AiProviderNames.OpenAi, "look"));
        IAiProviderRouter router = new AiProviderRouter([first, second]);

        AiRoutingResult result = await router.RouteAsync(new AiParseRequest("inspect room"));

        Assert.True(result.HasCommand);
        Assert.Equal("look", result.CommandText);
        Assert.Equal(AiProviderNames.OpenAi, result.ProviderName);
        Assert.Equal(2, result.Attempts.Count);
    }

    [Fact]
    public async Task Router_SkipsProviderWhenBudgetExceeded()
    {
        InMemoryDailyTokenBudgetPolicy budget = new();
        _ = budget.SetDailyLimit(AiProviderNames.OneMinAi, 10);

        IAiCommandProvider first = new StubProvider(AiProviderNames.OneMinAi, AiProviderResult.Success(AiProviderNames.OneMinAi, "look"));
        IAiCommandProvider second = new StubProvider(AiProviderNames.Ollama, AiProviderResult.Success(AiProviderNames.Ollama, "look"));
        IAiProviderRouter router = new AiProviderRouter([first, second], budget);

        AiRoutingResult result = await router.RouteAsync(new AiParseRequest("look", EstimatedTokens: 20));

        Assert.True(result.HasCommand);
        Assert.Equal(AiProviderNames.Ollama, result.ProviderName);
        Assert.Equal(AiAttemptOutcome.SkippedBudget, result.Attempts[0].Outcome);
    }

    [Fact]
    public void Parser_UsesAiCommand_WhenSafeAndValid()
    {
        IAiProviderRouter router = new StubRouter(AiRoutingResult.Success(
            AiProviderNames.Ollama,
            "go north",
            [new AiProviderAttempt(AiProviderNames.Ollama, AiAttemptOutcome.Success)]));

        ICommandParser fallback = new KeywordParser(KeywordParserConfig.Default);
        OllamaCommandParser parser = new(
            router,
            fallback,
            new CommandAllowlistSafetyPolicy(),
            new AiParserOptions());

        ICommand command = parser.Parse("head out");

        Assert.IsType<GoCommand>(command);
    }

    [Fact]
    public void Parser_FallsBack_WhenAiCommandRejectedBySafety()
    {
        IAiProviderRouter router = new StubRouter(AiRoutingResult.Success(
            AiProviderNames.Ollama,
            "dance wildly",
            [new AiProviderAttempt(AiProviderNames.Ollama, AiAttemptOutcome.Success)]));

        ICommandParser fallback = new KeywordParser(KeywordParserConfig.Default);
        OllamaCommandParser parser = new(
            router,
            fallback,
            new CommandAllowlistSafetyPolicy(),
            new AiParserOptions { StrictMode = false, FallbackOnSafetyRejection = true });

        ICommand command = parser.Parse("look");

        Assert.IsType<LookCommand>(command);
    }

    [Fact]
    public void Parser_StrictMode_ReturnsParserError_WhenUnsafeCommand()
    {
        IAiProviderRouter router = new StubRouter(AiRoutingResult.Success(
            AiProviderNames.Ollama,
            "dance wildly",
            [new AiProviderAttempt(AiProviderNames.Ollama, AiAttemptOutcome.Success)]));

        ICommandParser fallback = new KeywordParser(KeywordParserConfig.Default);
        OllamaCommandParser parser = new(
            router,
            fallback,
            new CommandAllowlistSafetyPolicy(),
            new AiParserOptions { StrictMode = true, FallbackOnSafetyRejection = false });

        ICommand command = parser.Parse("look");

        Assert.IsType<ParserErrorCommand>(command);
    }

    [Fact]
    public void Parser_PreferLocalCommandFirst_SkipsAiForKnownCommand()
    {
        StubRouter router = new(AiRoutingResult.Success(
            AiProviderNames.Ollama,
            "look",
            [new AiProviderAttempt(AiProviderNames.Ollama, AiAttemptOutcome.Success)]));

        ICommandParser fallback = new KeywordParser(KeywordParserConfig.Default);
        AiCommandParser parser = new(
            router,
            fallback,
            new CommandAllowlistSafetyPolicy(),
            new AiParserOptions { PreferLocalCommandFirst = true });

        ICommand command = parser.Parse("take all");

        Assert.IsType<TakeAllCommand>(command);
        Assert.Equal(0, router.CallCount);
    }

    private sealed class StubProvider(string name, AiProviderResult response) : IAiCommandProvider
    {
        public string Name { get; } = name;

        public Task<AiProviderResult> ParseAsync(AiParseRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(response);
        }
    }

    private sealed class StubRouter(AiRoutingResult response) : IAiProviderRouter
    {
        public int CallCount { get; private set; }

        public Task<AiRoutingResult> RouteAsync(AiParseRequest request, CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(response);
        }
    }
}

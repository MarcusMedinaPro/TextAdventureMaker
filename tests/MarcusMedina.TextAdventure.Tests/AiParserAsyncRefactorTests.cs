// <copyright file="AiParserAsyncRefactorTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.AI;
using MarcusMedina.TextAdventure.AI.Contracts;
using MarcusMedina.TextAdventure.AI.Models;
using MarcusMedina.TextAdventure.AI.Policies;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.Tests;

public class AiParserAsyncRefactorTests
{
    [Fact]
    public async Task ParseAsync_UsesAiCommand_WhenAvailable()
    {
        IAiProviderRouter router = new StubRouter(AiRoutingResult.Success(
            AiProviderNames.OpenAi,
            "go north",
            [new AiProviderAttempt(AiProviderNames.OpenAi, AiAttemptOutcome.Success)]));

        ICommandParser fallback = new KeywordParser(KeywordParserConfig.Default);
        AiCommandParser parser = new(
            router,
            fallback,
            new CommandAllowlistSafetyPolicy(),
            new AiParserOptions { PreferLocalCommandFirst = false });

        ICommand command = await parser.ParseAsync("head out", TestContext.Current.CancellationToken);

        Assert.IsType<GoCommand>(command);
    }

    private sealed class StubRouter(AiRoutingResult response) : IAiProviderRouter
    {
        public Task<AiRoutingResult> RouteAsync(AiParseRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(response);
        }
    }
}

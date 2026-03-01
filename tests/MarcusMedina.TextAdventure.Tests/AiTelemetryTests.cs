// <copyright file="AiTelemetryTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.AI.Models;
using MarcusMedina.TextAdventure.AI.Policies;
using MarcusMedina.TextAdventure.AI.Router;
using MarcusMedina.TextAdventure.AI.Contracts;

namespace MarcusMedina.TextAdventure.Tests;

public class AiTelemetryTests
{
    [Fact]
    public async Task Router_RecordsTelemetryEvents()
    {
        InMemoryAiTelemetrySink telemetry = new();
        IAiCommandProvider provider = new StubProvider(
            AiProviderNames.OpenAi,
            AiProviderResult.Success(AiProviderNames.OpenAi, "look", new AiTokenUsage(2, 3)));

        IAiProviderRouter router = new AiProviderRouter([provider], telemetrySink: telemetry);
        _ = await router.RouteAsync(new AiParseRequest("inspect room"), TestContext.Current.CancellationToken);

        Assert.True(telemetry.EventCounts.TryGetValue("route.attempt", out int count));
        Assert.Equal(1, count);
        Assert.Equal(5, telemetry.TotalTokens);
    }

    [Fact]
    public async Task Router_RecordsBudgetSkipTelemetry()
    {
        InMemoryDailyTokenBudgetPolicy budget = new();
        InMemoryAiTelemetrySink telemetry = new();
        _ = budget.SetDailyLimit(AiProviderNames.OneMinAi, 10);

        IAiCommandProvider first = new StubProvider(AiProviderNames.OneMinAi, AiProviderResult.Success(AiProviderNames.OneMinAi, "look"));
        IAiCommandProvider second = new StubProvider(AiProviderNames.Ollama, AiProviderResult.Success(AiProviderNames.Ollama, "look"));
        IAiProviderRouter router = new AiProviderRouter([first, second], budget, telemetry);

        _ = await router.RouteAsync(new AiParseRequest("look", EstimatedTokens: 20), TestContext.Current.CancellationToken);

        Assert.True(telemetry.EventCounts.TryGetValue("route.skip_budget", out int skipCount));
        Assert.Equal(1, skipCount);
    }

    private sealed class StubProvider(string name, AiProviderResult response) : IAiCommandProvider
    {
        public string Name { get; } = name;

        public Task<AiProviderResult> ParseAsync(AiParseRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(response);
        }
    }
}

// <copyright file="AiDescriptionCacheTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.AI.Contracts;
using MarcusMedina.TextAdventure.AI.Features;
using MarcusMedina.TextAdventure.AI.Models;
using MarcusMedina.TextAdventure.AI.Plugin;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.Tests;

public class AiDescriptionCacheTests
{
    [Fact]
    public void Run_RepeatedLookAtNpc_UsesCachedAiDescription()
    {
        CountingRouter router = new("A stern watchman scans the square.");
        (Game game, StringWriter output) = CreateGame(router, "look watchman\nlook watchman\nquit\n");

        game.Run();

        Assert.Equal(1, router.RequestCount);
        Assert.Contains("A stern watchman scans the square.", output.ToString());
    }

    [Fact]
    public void Run_LookCacheClearsAfterSuccessfulWorldChange()
    {
        CountingRouter router = new("A stern watchman scans the square.");
        string input = "look watchman\ngo north\ngo south\nlook watchman\nquit\n";
        (Game game, _) = CreateGame(router, input);

        game.Run();

        Assert.Equal(2, router.RequestCount);
    }

    [Fact]
    public void Run_LookAtNpc_WhenAiFails_CachesFallbackDescription()
    {
        FailingRouter router = new();
        (Game game, StringWriter output) = CreateGame(router, "look watchman\nlook watchman\nquit\n");

        game.Run();

        Assert.Equal(1, router.RequestCount);
        Assert.Contains("A tired guard.", output.ToString());
    }

    [Fact]
    public async Task ItemDescription_SecondLookup_ReportsCacheHitViaDebugProbe()
    {
        CountingRouter router = new("A stern watchman scans the square.");
        SessionAiDescriptionCache cache = new();
        List<string> events = [];
        ItemDescriptionAiService service = new(
            router,
            cache,
            new AiParserOptions
            {
                DebugProbe = (eventName, payload) => events.Add($"{eventName}:{payload}")
            });

        DescriptionRequest request = new(
            EntityType: "npc",
            EntityId: "watchman",
            BaselinePrompt: "A tired guard.");

        _ = await service.GenerateDescriptionAsync(request, TestContext.Current.CancellationToken);
        _ = await service.GenerateDescriptionAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(1, router.RequestCount);
        Assert.Contains(events, e => e.StartsWith("feature.ai.call:", StringComparison.Ordinal));
        Assert.Contains(events, e => e.StartsWith("description.cache.hit:", StringComparison.Ordinal));
    }

    private static (Game game, StringWriter output) CreateGame(IAiProviderRouter router, string input)
    {
        Location square = new("square");
        Location inn = new("inn");
        square.AddExit(Direction.North, inn);
        inn.AddExit(Direction.South, square);

        square.AddNpc(new Npc("watchman", "Watchman").Description("A tired guard."));

        GameState state = new(square, worldLocations: [square, inn]);
        ICommandParser parser = new KeywordParser(KeywordParserConfigBuilder.BritishDefaults().Build());
        AiFeatureModule module = AiFeatureModule.Create(router, new SessionAiDescriptionCache(), new AiParserOptions());
        AiPluginOptions options = new()
        {
            EnableAiDialogue = false,
            EnableAiDescriptions = true,
            EnableAiDescriptionCacheInvalidation = true,
            EnableAiNpcMovement = false,
            EnableAiStoryDirector = false
        };

        ICommandParser pluginParser = parser.WithAiPlugin(module, options);
        StringReader reader = new(input);
        StringWriter writer = new();
        Game game = new(state, pluginParser, reader, writer);
        _ = game.EnableAiPluginRuntime(module, options);
        return (game, writer);
    }

    private sealed class CountingRouter(string responseText) : IAiProviderRouter
    {
        public int RequestCount { get; private set; }

        public Task<AiRoutingResult> RouteAsync(AiParseRequest request, CancellationToken cancellationToken = default)
        {
            RequestCount++;
            AiRoutingResult response = AiRoutingResult.Success(
                AiProviderNames.OpenAi,
                responseText,
                [new AiProviderAttempt(AiProviderNames.OpenAi, AiAttemptOutcome.Success)]);

            return Task.FromResult(response);
        }
    }

    private sealed class FailingRouter : IAiProviderRouter
    {
        public int RequestCount { get; private set; }

        public Task<AiRoutingResult> RouteAsync(AiParseRequest request, CancellationToken cancellationToken = default)
        {
            RequestCount++;
            AiRoutingResult response = AiRoutingResult.Failure(
                [new AiProviderAttempt(AiProviderNames.OpenAi, AiAttemptOutcome.Failed, "simulated failure")]);

            return Task.FromResult(response);
        }
    }
}

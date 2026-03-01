// <copyright file="AiRoutingProfileTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.AI;
using MarcusMedina.TextAdventure.AI.Contracts;
using MarcusMedina.TextAdventure.AI.Models;
using MarcusMedina.TextAdventure.AI.Policies;
using MarcusMedina.TextAdventure.AI.Providers;
using MarcusMedina.TextAdventure.AI.Router;
using MarcusMedina.TextAdventure.AI.Settings;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Parsing;

public class AiRoutingProfileTests
{
    [Fact]
    public void AiRoutingProfiles_DevLocalFirst_UsesExpectedProviderOrder()
    {
        IReadOnlyList<IAiCommandProvider> providers = AiRoutingProfiles.DevLocalFirst(
            new OllamaSettings { Enabled = false },
            new OneMinAiSettings(),
            new GeminiSettings());

        Assert.Equal(
            [typeof(OllamaCommandProvider), typeof(OneMinAiCommandProvider), typeof(GeminiCommandProvider)],
            providers.Select(p => p.GetType()).ToArray());
    }

    [Fact]
    public void AiRoutingProfiles_CloudLowCost_UsesExpectedProviderOrder()
    {
        IReadOnlyList<IAiCommandProvider> providers = AiRoutingProfiles.CloudLowCost(
            new OneMinAiSettings(),
            new GeminiSettings(),
            new OpenRouterSettings());

        Assert.Equal(
            [typeof(OneMinAiCommandProvider), typeof(GeminiCommandProvider), typeof(OpenRouterCommandProvider)],
            providers.Select(p => p.GetType()).ToArray());
    }

    [Fact]
    public void AiRoutingProfiles_CloudBalanced_UsesExpectedProviderOrder()
    {
        IReadOnlyList<IAiCommandProvider> providers = AiRoutingProfiles.CloudBalanced(
            new OpenAiSettings(),
            new ClaudeSettings(),
            new MistralSettings(),
            new OpenRouterSettings());

        Assert.Equal(
            [typeof(OpenAiCommandProvider), typeof(ClaudeCommandProvider), typeof(MistralCommandProvider), typeof(OpenRouterCommandProvider)],
            providers.Select(p => p.GetType()).ToArray());
    }

    [Fact]
    public void AiCommandParser_WithProfileProviders_FallsBackDeterministicallyWhenUnavailable()
    {
        IReadOnlyList<IAiCommandProvider> providers = AiRoutingProfiles.DevLocalFirst(
            new OllamaSettings { Enabled = false },
            new OneMinAiSettings(apiKey: ""),
            new GeminiSettings(apiKey: ""));

        IAiProviderRouter router = new AiProviderRouter(providers);
        ICommandParser fallback = new KeywordParser(KeywordParserConfig.Default);
        AiCommandParser parser = new(
            router,
            fallback,
            new CommandAllowlistSafetyPolicy(),
            new AiParserOptions { PreferLocalCommandFirst = false, StrictMode = false });

        ICommand command = parser.Parse("look");

        _ = Assert.IsType<LookCommand>(command);
        Assert.NotNull(parser.LastRoutingResult);
        Assert.False(parser.LastRoutingResult!.HasCommand);
        Assert.Equal(3, parser.LastRoutingResult.Attempts.Count);
    }
}

// <copyright file="AiRoutingProfiles.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.AI.Router;

public static class AiRoutingProfiles
{
    public static IReadOnlyList<IAiCommandProvider> DevLocalFirst(
        OllamaSettings ollama,
        OneMinAiSettings oneMinAi,
        GeminiSettings gemini)
    {
        return
        [
            new OllamaCommandProvider(ollama),
            new OneMinAiCommandProvider(oneMinAi),
            new GeminiCommandProvider(gemini)
        ];
    }

    public static IReadOnlyList<IAiCommandProvider> CloudLowCost(
        OneMinAiSettings oneMinAi,
        GeminiSettings gemini,
        OpenRouterSettings openRouter)
    {
        return
        [
            new OneMinAiCommandProvider(oneMinAi),
            new GeminiCommandProvider(gemini),
            new OpenRouterCommandProvider(openRouter)
        ];
    }

    public static IReadOnlyList<IAiCommandProvider> CloudBalanced(
        OpenAiSettings openAi,
        ClaudeSettings claude,
        MistralSettings mistral,
        OpenRouterSettings openRouter)
    {
        return
        [
            new OpenAiCommandProvider(openAi),
            new ClaudeCommandProvider(claude),
            new MistralCommandProvider(mistral),
            new OpenRouterCommandProvider(openRouter)
        ];
    }
}

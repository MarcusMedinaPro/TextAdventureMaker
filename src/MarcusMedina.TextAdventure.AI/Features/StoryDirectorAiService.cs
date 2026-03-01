// <copyright file="StoryDirectorAiService.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.AI.Features;

public sealed class StoryDirectorAiService(
    IAiProviderRouter router,
    AiParserOptions? options = null) : AiFeatureServiceBase(router, options), IStoryDirectorAiService
{
    public async Task<StoryEventProposal?> ProposeEventAsync(StoryDirectorContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        string prompt = AiFeaturePrompts.BuildStoryDirectorPrompt(context);
        AiRoutingResult? route = await TryRouteAsync(prompt, cancellationToken).ConfigureAwait(false);
        if (route is null || string.IsNullOrWhiteSpace(route.CommandText))
            return null;

        IReadOnlyDictionary<string, string> parsed = AiStructuredTextParser.ParseSegments(route.CommandText);
        string eventId = (parsed.GetValueOrDefault("event") ?? "event_auto").ToId();
        string summary = parsed.GetValueOrDefault("summary") ?? "Something shifts in the world.";
        string location = parsed.GetValueOrDefault("location") ?? context.CurrentLocationId;

        string? safeLocation = context.ConnectedLocations.FirstOrDefault(x => x.TextCompare(location))
            ?? (context.CurrentLocationId.TextCompare(location) ? context.CurrentLocationId : null);

        if (string.IsNullOrWhiteSpace(safeLocation))
            return null;

        IReadOnlyList<string> consequences = ParseConsequences(parsed.GetValueOrDefault("consequences"));
        return new StoryEventProposal(eventId, summary, safeLocation, consequences);
    }

    private static IReadOnlyList<string> ParseConsequences(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        return text
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
    }
}

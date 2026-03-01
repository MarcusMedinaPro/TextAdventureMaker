// <copyright file="ContextResolver.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Resolves references and ambiguities in parsed intents using game context.
/// </summary>
public sealed class ContextResolver
{
    /// <summary>
    /// Resolves pronouns, ambiguous references, and fuzzy matches in parsed intent.
    /// </summary>
    public ParsedIntent ResolveReferences(ParsedIntent intent, ParserContext context)
    {
        var resolved = intent;

        // Pronouns can be resolved through fuzzy matching or context
        // "it" / "that" / "this" → best match from visible objects
        if (resolved.DirectObject is "it" or "that" or "this")
        {
            var bestMatch = FindBestMatch(resolved.DirectObject, context.VisibleObjects.Concat(context.InventoryItems));
            if (bestMatch != null)
                resolved = resolved with { DirectObject = bestMatch };
        }

        // "him" / "her" / "them" → best match from NPCs
        if (resolved.IndirectObject is "him" or "her" or "them")
        {
            var bestMatch = FindBestMatch(resolved.IndirectObject, context.NpcNames);
            if (bestMatch != null)
                resolved = resolved with { IndirectObject = bestMatch };
        }

        // Fuzzy match object names
        if (resolved.DirectObject != null)
        {
            var bestMatch = FindBestMatch(resolved.DirectObject, context.VisibleObjects.Concat(context.InventoryItems));
            if (bestMatch != null)
                resolved = resolved with { DirectObject = bestMatch };
        }

        // Fuzzy match indirect object
        if (resolved.IndirectObject != null)
        {
            var bestMatch = FindBestMatch(resolved.IndirectObject, context.VisibleObjects.Concat(context.NpcNames));
            if (bestMatch != null)
                resolved = resolved with { IndirectObject = bestMatch };
        }

        return resolved;
    }

    private static string? FindBestMatch(string input, IEnumerable<string> candidates)
    {
        // Exact match
        var exact = candidates.FirstOrDefault(c => c.Equals(input, StringComparison.OrdinalIgnoreCase));
        if (exact != null)
            return exact;

        // Partial match (starts with)
        var partial = candidates.FirstOrDefault(c => c.StartsWith(input, StringComparison.OrdinalIgnoreCase));
        if (partial != null)
            return partial;

        // Contains match
        var contains = candidates.FirstOrDefault(c => c.Contains(input, StringComparison.OrdinalIgnoreCase));
        if (contains != null)
            return contains;

        // Fuzzy match using Levenshtein distance
        return candidates
            .Select(c => (candidate: c, distance: LevenshteinDistance(input, c)))
            .Where(x => x.distance <= 2)
            .OrderBy(x => x.distance)
            .Select(x => x.candidate)
            .FirstOrDefault();
    }

    private static int LevenshteinDistance(string a, string b)
    {
        a = a.ToLowerInvariant();
        b = b.ToLowerInvariant();

        if (a.Length == 0)
            return b.Length;
        if (b.Length == 0)
            return a.Length;

        var distances = new int[a.Length + 1, b.Length + 1];

        for (int i = 0; i <= a.Length; i++)
            distances[i, 0] = i;

        for (int j = 0; j <= b.Length; j++)
            distances[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                distances[i, j] = Math.Min(
                    Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1),
                    distances[i - 1, j - 1] + cost
                );
            }
        }

        return distances[a.Length, b.Length];
    }
}

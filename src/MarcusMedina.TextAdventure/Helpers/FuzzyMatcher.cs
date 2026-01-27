// <copyright file="FuzzyMatcher.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System.Linq;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Helpers;

public static class FuzzyMatcher
{
    /// <summary>Common command words used to avoid fuzzy matching against intents.</summary>
    private static readonly HashSet<string> DefaultStopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "look",
        "examine",
        "take",
        "get",
        "pickup",
        "pick",
        "drop",
        "use",
        "open",
        "close",
        "unlock",
        "lock",
        "go",
        "move",
        "talk",
        "speak",
        "attack",
        "fight",
        "run",
        "flee",
        "read",
        "combine",
        "mix",
        "pour",
        "save",
        "load",
        "inventory",
        "inv",
        "stats",
        "health",
        "quit",
        "exit"
    };

    /// <summary>Return true when the token looks like a command word.</summary>
    public static bool IsLikelyCommandToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        return DefaultStopWords.Contains(token.Trim());
    }

    /// <summary>Find the closest token within the maximum distance, or null if none/ambiguous.</summary>
    public static string? FindBestToken(string input, IEnumerable<string> candidates, int maxDistance)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        if (candidates == null) return null;

        var bestDistance = int.MaxValue;
        string? best = null;

        foreach (var candidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(candidate)) continue;
            var distance = input.FuzzyDistanceTo(candidate, maxDistance);
            if (distance > maxDistance) continue;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = candidate;
            }
            else if (distance == bestDistance)
            {
                best = null; // ambiguous
            }
        }

        return best;
    }

    /// <summary>Find the closest item by name/id/aliases within the maximum distance.</summary>
    public static IItem? FindBestItem(IEnumerable<IItem> items, string input, int maxDistance)
    {
        return FindBestEntity(items, input, maxDistance, item =>
            new[] { item.Name, item.Id }.Concat(item.Aliases));
    }

    /// <summary>Find the closest NPC by name/id within the maximum distance.</summary>
    public static INpc? FindBestNpc(IEnumerable<INpc> npcs, string input, int maxDistance)
    {
        return FindBestEntity(npcs, input, maxDistance, npc =>
            new[] { npc.Name, npc.Id });
    }

    /// <summary>Find the closest door by name/id/aliases within the maximum distance.</summary>
    public static IDoor? FindBestDoor(IEnumerable<IDoor> doors, string input, int maxDistance)
    {
        return FindBestEntity(doors, input, maxDistance, door =>
            new[] { door.Name, door.Id }.Concat(door.Aliases));
    }

    private static T? FindBestEntity<T>(
        IEnumerable<T> entities,
        string input,
        int maxDistance,
        Func<T, IEnumerable<string>> tokens)
        where T : class
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        var bestDistance = int.MaxValue;
        T? best = null;

        foreach (var entity in entities)
        {
            var candidates = tokens(entity);
            if (candidates == null) continue;

            var distance = GetBestDistance(input, candidates, maxDistance);
            if (distance > maxDistance) continue;

            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = entity;
            }
            else if (distance == bestDistance)
            {
                best = null; // ambiguous
            }
        }

        return best;
    }

    private static int GetBestDistance(string input, IEnumerable<string> candidates, int maxDistance)
    {
        var best = int.MaxValue;
        foreach (var candidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(candidate)) continue;
            var distance = input.FuzzyDistanceTo(candidate, maxDistance);
            if (distance < best) best = distance;
        }
        return best;
    }
}

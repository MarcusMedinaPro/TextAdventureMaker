// <copyright file="CommandSuggester.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Generates command suggestions based on partial input and game context.
/// </summary>
public sealed class CommandSuggester
{
    /// <summary>
    /// Gets intelligent command suggestions for autocomplete.
    /// </summary>
    public IEnumerable<string> GetSuggestions(string partialInput, ParserContext context)
    {
        var suggestions = new List<string>();

        if (string.IsNullOrWhiteSpace(partialInput))
        {
            // Suggest common commands
            suggestions.AddRange(["look", "inventory", "go north"]);

            // Suggest context-aware commands
            if (context.VisibleObjects.Any())
                suggestions.Add($"take {context.VisibleObjects.First()}");
            if (context.NpcNames.Any())
                suggestions.Add($"talk to {context.NpcNames.First()}");

            return suggestions.Take(5);
        }

        var words = partialInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var verb = words[0].ToLowerInvariant();

        // Autocomplete verb
        if (words.Length == 1)
        {
            return context.AvailableVerbs
                .Where(v => v.StartsWith(verb, StringComparison.OrdinalIgnoreCase))
                .Take(5);
        }

        // Autocomplete object based on verb
        if (words.Length == 2)
        {
            var partial = words[1];
            var candidates = verb switch
            {
                "take" or "get" or "grab" => context.VisibleObjects,
                "drop" or "put" => context.InventoryItems,
                "talk" or "ask" or "speak" => context.NpcNames,
                "go" or "walk" or "move" => ["north", "south", "east", "west", "up", "down"],
                _ => context.VisibleObjects.Concat(context.InventoryItems)
            };

            return candidates
                .Where(c => c.StartsWith(partial, StringComparison.OrdinalIgnoreCase))
                .Select(c => $"{verb} {c}")
                .Take(5);
        }

        return suggestions;
    }
}

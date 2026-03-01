// <copyright file="AmbiguityResolver.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Resolves ambiguities when multiple objects match the player's input.
/// </summary>
public sealed class AmbiguityResolver
{
    /// <summary>
    /// Attempts to resolve an object name to a single match, or returns options if ambiguous.
    /// </summary>
    public DisambiguationResult Resolve(string objectName, ParserContext context)
    {
        var matches = context.VisibleObjects
            .Concat(context.InventoryItems)
            .Where(o => o.Contains(objectName, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToList();

        return matches.Count switch
        {
            0 => new DisambiguationResult(false, null, $"You don't see any '{objectName}' here."),
            1 => new DisambiguationResult(true, matches[0], null),
            _ => new DisambiguationResult(false, null,
                $"Which do you mean: {string.Join(" or ", matches)}?",
                matches)
        };
    }
}

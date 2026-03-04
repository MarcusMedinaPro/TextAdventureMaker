// <copyright file="FuzzyItemResolver.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Helpers;

using MarcusMedina.TextAdventure.Interfaces;

public static class FuzzyItemResolver
{
    public static (IItem? item, string? suggestion) Resolve(
        IGameState state, IEnumerable<IItem> source, IItem? current, string name)
    {
        if (current is not null || !state.EnableFuzzyMatching || FuzzyMatcher.IsLikelyCommandToken(name))
            return (current, null);
        IItem? best = FuzzyMatcher.FindBestItem(source, name, state.FuzzyMaxDistance);
        return best is not null ? (best, best.Name) : (null, null);
    }
}

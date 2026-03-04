// <copyright file="EnglishGrammar.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Localization;

public sealed class EnglishGrammar : IGrammarProvider
{
    public string WithArticle(string noun)
    {
        if (string.IsNullOrWhiteSpace(noun))
            return string.Empty;

        string trimmed = noun.Trim();
        char first = char.ToLowerInvariant(trimmed[0]);
        string article = "aeiou".Contains(first) ? "an" : "a";
        return $"{article} {trimmed}";
    }

    public string Plural(string noun, int count)
    {
        if (string.IsNullOrWhiteSpace(noun))
            return $"{count}";

        string trimmed = noun.Trim();
        if (count == 1)
            return $"{count} {trimmed}";

        string plural = trimmed.EndsWith("s", StringComparison.OrdinalIgnoreCase)
            ? trimmed
            : $"{trimmed}s";

        return $"{count} {plural}";
    }

    public string NaturalList(IEnumerable<string> items)
    {
        if (items is null)
            return string.Empty;

        List<string> list = items.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();
        return list.Count switch
        {
            0 => string.Empty,
            1 => list[0],
            2 => $"{list[0]} and {list[1]}",
            _ => $"{string.Join(", ", list.Take(list.Count - 1))}, and {list.Last()}"
        };
    }
}

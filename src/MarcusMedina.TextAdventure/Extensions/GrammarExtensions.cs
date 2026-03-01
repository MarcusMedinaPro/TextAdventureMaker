// <copyright file="GrammarExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Extensions;

public static class GrammarExtensions
{
    public static IGrammarProvider Provider { get; set; } = new EnglishGrammar();

    public static string WithArticle(this string noun)
    {
        return Provider.WithArticle(noun);
    }

    public static string Plural(this string noun, int count)
    {
        return Provider.Plural(noun, count);
    }

    public static string NaturalList(this IEnumerable<string> items)
    {
        return Provider.NaturalList(items);
    }
}

public sealed class EnglishGrammar : IGrammarProvider
{
    public string WithArticle(string noun)
    {
        if (string.IsNullOrWhiteSpace(noun))
        {
            return string.Empty;
        }

        string trimmed = noun.Trim();
        char first = char.ToLowerInvariant(trimmed[0]);
        string article = "a";
        if ("aeiou".Contains(first))
        {
            article = "an";
        }

        return $"{article} {trimmed}";
    }

    public string Plural(string noun, int count)
    {
        if (string.IsNullOrWhiteSpace(noun))
        {
            return $"{count}";
        }

        string trimmed = noun.Trim();
        if (count == 1)
        {
            return $"{count} {trimmed}";
        }

        string plural = trimmed.EndsWith("s", StringComparison.OrdinalIgnoreCase)
            ? trimmed
            : $"{trimmed}s";

        return $"{count} {plural}";
    }

    public string NaturalList(IEnumerable<string> items)
    {
        if (items == null)
        {
            return string.Empty;
        }

        List<string> list = items.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();
        if (list.Count == 0)
        {
            return string.Empty;
        }

        if (list.Count == 1)
        {
            return list[0];
        }

        if (list.Count == 2)
        {
            return $"{list[0]} and {list[1]}";
        }

        string head = string.Join(", ", list.Take(list.Count - 1));
        return $"{head}, and {list.Last()}";
    }
}

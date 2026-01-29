// <copyright file="StringExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Extensions;

using System.Collections.Generic;
using System.Globalization;
using System.Text;

public static class StringExtensions
{
    /// <summary>Return the lower-cased, trimmed string or an empty string if null/whitespace.</summary>
    public static string Lower(this string? text) => string.IsNullOrWhiteSpace(text) ? string.Empty : text.Trim().ToLowerInvariant();

    /// <summary>Case-insensitive comparison after trimming.</summary>
    public static bool TextCompare(this string? text, string? other) => !string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(other) && string.Equals(text.Trim(), other.Trim(), StringComparison.OrdinalIgnoreCase);

    /// <summary>Fluent alias for case-insensitive comparison.</summary>
    public static bool Is(this string? text, string? other) => text.TextCompare(other);

    /// <summary>Convert a string to a stable identifier (lowercase, underscores).</summary>
    public static string ToId(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var builder = new StringBuilder(text.Length);
        var previousUnderscore = false;

        foreach (var ch in text.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                _ = builder.Append(ch);
                previousUnderscore = false;
                continue;
            }

            if (ch == '_' || char.IsWhiteSpace(ch) || ch == '-')
            {
                if (!previousUnderscore && builder.Length > 0)
                {
                    _ = builder.Append('_');
                    previousUnderscore = true;
                }
            }
        }

        return builder.ToString().Trim('_');
    }

    /// <summary>Convert to title case using invariant culture.</summary>
    public static string ToProperCase(this string? text) => string.IsNullOrWhiteSpace(text)
            ? text ?? string.Empty
            : CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text.ToLowerInvariant());

    /// <summary>Capitalise the first letter and lower-case the rest.</summary>
    public static string ToSentenceCase(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text ?? string.Empty;

        var lower = text.ToLowerInvariant();
        var chars = lower.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            if (!char.IsLetter(chars[i]))
                continue;
            chars[i] = char.ToUpperInvariant(chars[i]);
            break;
        }

        return new string(chars);
    }

    /// <summary>Randomise casing per letter using <see cref="Random.Shared"/>.</summary>
    public static string ToCrazyCaps(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text ?? string.Empty;

        var builder = new StringBuilder(text.Length);
        foreach (var ch in text)
        {
            if (!char.IsLetter(ch))
            {
                _ = builder.Append(ch);
                continue;
            }

            _ = builder.Append(Random.Shared.Next(2) == 0
                ? char.ToLowerInvariant(ch)
                : char.ToUpperInvariant(ch));
        }

        return builder.ToString();
    }

    /// <summary>Return an approximate distance using Levenshtein with collapsed repeats.</summary>
    public static int FuzzyDistanceTo(this string? text, string? other, int maxDistance = 1)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(other))
            return maxDistance + 1;

        var left = text.Trim().ToLowerInvariant();
        var right = other.Trim().ToLowerInvariant();
        if (left.TextCompare(right))
            return 0;

        var leftCollapsed = left.CollapseRepeats();
        var rightCollapsed = right.CollapseRepeats();

        var best = left.LevenshteinDistanceTo(right, maxDistance);
        best = Math.Min(best, leftCollapsed.LevenshteinDistanceTo(right, maxDistance));
        best = Math.Min(best, left.LevenshteinDistanceTo(rightCollapsed, maxDistance));

        return best;
    }

    /// <summary>Collapse consecutive repeated characters (e.g. "loook" → "lok").</summary>
    public static string CollapseRepeats(this string? text)
    {
        if (string.IsNullOrEmpty(text))
            return text ?? string.Empty;
        var chars = new List<char>(text.Length);
        var last = '\0';
        foreach (var ch in text)
        {
            if (ch == last)
                continue;
            chars.Add(ch);
            last = ch;
        }

        return new string(chars.ToArray());
    }

    /// <summary>Return the Levenshtein distance with no maximum bound.</summary>
    public static int LevenshteinDistanceTo(this string? text, string? other) => LevenshteinDistanceTo(text, other, int.MaxValue);

    /// <summary>Return the Levenshtein distance, stopping early above <paramref name="maxDistance"/>.</summary>
    public static int LevenshteinDistanceTo(this string? text, string? other, int maxDistance) => string.IsNullOrWhiteSpace(text)
            ? string.IsNullOrWhiteSpace(other) ? 0 : other!.Length
            : string.IsNullOrWhiteSpace(other) ? text!.Length : LevenshteinDistanceCore(text.Trim(), other.Trim(), maxDistance);

    /// <summary>Friendly wrapper over <see cref="LevenshteinDistanceTo(string?,string?)"/>.</summary>
    public static int SimilarTo(this string? text, string? other) => text.LevenshteinDistanceTo(other);

    /// <summary>Return true when the fuzzy distance is within <paramref name="maxDistance"/>.</summary>
    public static bool FuzzyMatch(this string? text, string? other, int maxDistance = 1) => text.FuzzyDistanceTo(other, maxDistance) <= maxDistance;

    /// <summary>Return a Soundex phonetic key.</summary>
    public static string SoundexKey(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var input = text.Trim().ToUpperInvariant();
        var first = input[0];

        var builder = new StringBuilder(4);
        _ = builder.Append(first);

        var previousCode = SoundexCode(first);
        for (var i = 1; i < input.Length && builder.Length < 4; i++)
        {
            var code = SoundexCode(input[i]);
            if (code == '0')
                continue;
            if (code == previousCode)
                continue;
            _ = builder.Append(code);
            previousCode = code;
        }

        while (builder.Length < 4)
        {
            _ = builder.Append('0');
        }

        return builder.ToString();
    }

    /// <summary>Return true when Soundex keys match.</summary>
    public static bool SoundsLike(this string? text, string? other) => !string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(other) && text.SoundexKey().TextCompare(other.SoundexKey());

    private static int LevenshteinDistanceCore(string a, string b, int maxDistance)
    {
        if (a.Length == 0)
            return b.Length;
        if (b.Length == 0)
            return a.Length;
        if (Math.Abs(a.Length - b.Length) > maxDistance)
            return maxDistance + 1;

        var previous = new int[b.Length + 1];
        var current = new int[b.Length + 1];

        for (var j = 0; j <= b.Length; j++)
        {
            previous[j] = j;
        }

        for (var i = 1; i <= a.Length; i++)
        {
            current[0] = i;
            var minInRow = current[0];
            var aChar = a[i - 1];

            for (var j = 1; j <= b.Length; j++)
            {
                var cost = aChar == b[j - 1] ? 0 : 1;
                current[j] = Math.Min(
                    Math.Min(current[j - 1] + 1, previous[j] + 1),
                    previous[j - 1] + cost);

                if (current[j] < minInRow)
                    minInRow = current[j];
            }

            if (minInRow > maxDistance)
                return maxDistance + 1;

            (current, previous) = (previous, current);
        }

        return previous[b.Length];
    }

    private static char SoundexCode(char ch) => ch switch
    {
        'B' or 'F' or 'P' or 'V' => '1',
        'C' or 'G' or 'J' or 'K' or 'Q' or 'S' or 'X' or 'Z' => '2',
        'D' or 'T' => '3',
        'L' => '4',
        'M' or 'N' => '5',
        'R' => '6',
        _ => '0'
    };
}

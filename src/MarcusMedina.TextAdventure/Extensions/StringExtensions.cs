// <copyright file="StringExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System.Globalization;
using System.Text;

namespace MarcusMedina.TextAdventure.Extensions;

public static class StringExtensions
{
    public static string Lower(this string? text)
    {
        return string.IsNullOrWhiteSpace(text) ? string.Empty : text.Trim().ToLowerInvariant();
    }

    public static bool TextCompare(this string? text, string? other)
    {
        if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(other)) return false;
        return string.Equals(text.Trim(), other.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    public static string ToId(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        var builder = new StringBuilder(text.Length);
        var previousUnderscore = false;

        foreach (var ch in text.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(ch);
                previousUnderscore = false;
                continue;
            }

            if (ch == '_' || char.IsWhiteSpace(ch) || ch == '-')
            {
                if (!previousUnderscore && builder.Length > 0)
                {
                    builder.Append('_');
                    previousUnderscore = true;
                }
            }
        }

        return builder.ToString().Trim('_');
    }

    public static string ToProperCase(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text ?? string.Empty;
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(text.ToLowerInvariant());
    }

    public static string ToSentenceCase(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text ?? string.Empty;

        var lower = text.ToLowerInvariant();
        var chars = lower.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            if (!char.IsLetter(chars[i])) continue;
            chars[i] = char.ToUpperInvariant(chars[i]);
            break;
        }

        return new string(chars);
    }

    public static string ToCrazyCaps(this string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text ?? string.Empty;

        var builder = new StringBuilder(text.Length);
        foreach (var ch in text)
        {
            if (!char.IsLetter(ch))
            {
                builder.Append(ch);
                continue;
            }

            builder.Append(Random.Shared.Next(2) == 0
                ? char.ToLowerInvariant(ch)
                : char.ToUpperInvariant(ch));
        }

        return builder.ToString();
    }
}

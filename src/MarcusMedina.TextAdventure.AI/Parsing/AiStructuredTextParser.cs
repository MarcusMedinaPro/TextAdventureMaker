// <copyright file="AiStructuredTextParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.AI.Parsing;

public static class AiStructuredTextParser
{
    public static IReadOnlyDictionary<string, string> ParseSegments(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string[] segments = text.Split(';', StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, string> result = new(StringComparer.OrdinalIgnoreCase);

        foreach (string segment in segments)
        {
            int colon = segment.IndexOf(':');
            if (colon <= 0 || colon >= segment.Length - 1)
                continue;

            string key = segment[..colon].Trim().ToId();
            string value = segment[(colon + 1)..].Trim();

            if (!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(value))
                result[key] = value;
        }

        return result;
    }

    public static int ParseIntOrDefault(string? text, int min, int max, int defaultValue = 0)
    {
        if (!int.TryParse(text, out int value))
            return defaultValue;

        return value.Clamp(min, max);
    }
}

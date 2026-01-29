// <copyright file="DslParseError.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// Represents a DSL parsing error with helpful context.
/// </summary>
public class DslParseError(int line, string lineContent, string message, string? suggestion = null, int? column = null)
{
    /// <summary>
    /// Line number where the error occurred (1-based).
    /// </summary>
    public int Line { get; } = line;

    /// <summary>
    /// Column where the error occurred (1-based), if known.
    /// </summary>
    public int? Column { get; } = column;

    /// <summary>
    /// The problematic line content.
    /// </summary>
    public string LineContent { get; } = lineContent;

    /// <summary>
    /// The error message.
    /// </summary>
    public string Message { get; } = message;

    /// <summary>
    /// Suggestion for how to fix the error.
    /// </summary>
    public string? Suggestion { get; } = suggestion;

    public override string ToString()
    {
        var result = $"Line {Line}: {Message}";
        if (!string.IsNullOrWhiteSpace(Suggestion))
        {
            result += $"\n  Suggestion: {Suggestion}";
        }

        result += $"\n  > {LineContent}";
        if (Column.HasValue)
        {
            result += $"\n    {new string(' ', Column.Value - 1)}^";
        }

        return result;
    }
}

/// <summary>
/// Exception thrown when DSL parsing fails.
/// </summary>
public class DslParseException(IReadOnlyList<DslParseError> errors) : Exception(FormatMessage(errors))
{
    /// <summary>
    /// The parsing errors encountered.
    /// </summary>
    public IReadOnlyList<DslParseError> Errors { get; } = errors;

    public DslParseException(DslParseError error)
        : this(new[] { error })
    {
    }

    private static string FormatMessage(IReadOnlyList<DslParseError> errors) => errors.Count == 0
            ? "DSL parsing failed."
            : errors.Count == 1
            ? $"DSL parsing error:\n{errors[0]}"
            : $"DSL parsing failed with {errors.Count} errors:\n" +
               string.Join("\n\n", errors.Select(e => e.ToString()));
}

/// <summary>
/// Helper for generating friendly DSL error messages.
/// </summary>
public static class DslErrorHelper
{
    private static readonly HashSet<string> ValidKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "world", "goal", "start", "location", "description", "item", "key", "door", "exit"
    };

    /// <summary>
    /// Suggests a correction for a misspelled keyword.
    /// </summary>
    public static string? SuggestKeyword(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var (Keyword, Distance) = ValidKeywords
            .Select(k => (Keyword: k, Distance: LevenshteinDistance(input.ToLowerInvariant(), k)))
            .Where(x => x.Distance <= 3)
            .OrderBy(x => x.Distance)
            .FirstOrDefault();

        return Keyword;
    }

    /// <summary>
    /// Creates an error for an unknown keyword.
    /// </summary>
    public static DslParseError UnknownKeyword(int line, string lineContent, string keyword)
    {
        var suggestion = SuggestKeyword(keyword);
        var suggestionText = suggestion != null
            ? $"Did you mean '{suggestion}'?"
            : $"Valid keywords: {string.Join(", ", ValidKeywords.OrderBy(k => k))}";

        return new DslParseError(
            line,
            lineContent,
            $"Unknown keyword: '{keyword}'",
            suggestionText,
            1);
    }

    /// <summary>
    /// Creates an error for missing location context.
    /// </summary>
    public static DslParseError NoCurrentLocation(int line, string lineContent, string keyword) => new(
            line,
            lineContent,
            $"'{keyword}' requires a location context",
            "Add a 'location:' line before this");

    /// <summary>
    /// Creates an error for invalid exit syntax.
    /// </summary>
    public static DslParseError InvalidExitSyntax(int line, string lineContent) => new(
            line,
            lineContent,
            "Invalid exit syntax",
            "Use format: exit: direction -> target (e.g., 'exit: north -> forest')");

    /// <summary>
    /// Creates an error for missing required field.
    /// </summary>
    public static DslParseError MissingField(int line, string lineContent, string keyword, string field) => new(
            line,
            lineContent,
            $"'{keyword}' is missing required field: {field}",
            $"Format: {keyword}: {GetFormatExample(keyword)}");

    private static string GetFormatExample(string keyword) => keyword.ToLowerInvariant() switch
    {
        "location" => "id | description",
        "item" => "id | name | description",
        "key" => "id | name | description",
        "door" => "id | name | description | key=keyid",
        "exit" => "direction -> target | door=doorid | oneway",
        _ => "value"
    };

    private static int LevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a))
            return b?.Length ?? 0;
        if (string.IsNullOrEmpty(b))
            return a.Length;

        var dp = new int[a.Length + 1, b.Length + 1];

        for (var i = 0; i <= a.Length; i++)
            dp[i, 0] = i;
        for (var j = 0; j <= b.Length; j++)
            dp[0, j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            for (var j = 1; j <= b.Length; j++)
            {
                var cost = a[i - 1] == b[j - 1] ? 0 : 1;
                dp[i, j] = Math.Min(
                    Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                    dp[i - 1, j - 1] + cost);
            }
        }

        return dp[a.Length, b.Length];
    }
}

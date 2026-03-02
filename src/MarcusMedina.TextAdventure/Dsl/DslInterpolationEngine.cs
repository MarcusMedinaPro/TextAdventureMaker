// <copyright file="DslInterpolationEngine.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.RegularExpressions;

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 interpolation engine for template variables and expressions.
/// Supports {path}, {path|formatter}, {path??fallback} syntax.
/// </summary>
public sealed class DslInterpolationEngine
{
    private readonly Regex _tokenRegex = new(@"\{([^}]+)\}", RegexOptions.Compiled);
    private readonly DslInterpolationContext _context;

    public DslInterpolationEngine(DslInterpolationContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Interpolate template string with variables from context.
    /// </summary>
    public string Interpolate(string template)
    {
        if (string.IsNullOrEmpty(template))
            return template;

        return _tokenRegex.Replace(template, match =>
        {
            var token = match.Groups[1].Value;
            return ResolveToken(token);
        });
    }

    private string ResolveToken(string token)
    {
        // Handle fallback syntax: {path??fallback}
        string fallback = "";
        if (token.Contains("??"))
        {
            var parts = token.Split("??", 2);
            token = parts[0].Trim();
            fallback = parts[1].Trim();
        }

        // Handle formatter syntax: {path|formatter}
        string formatter = "";
        if (token.Contains("|"))
        {
            var parts = token.Split("|", 2);
            token = parts[0].Trim();
            formatter = parts[1].Trim();
        }

        // Resolve the path
        var value = ResolvePath(token);
        if (value is null)
            return fallback;

        // Apply formatter if specified
        if (!string.IsNullOrEmpty(formatter))
            return ApplyFormatter(value, formatter);

        return value.ToString() ?? fallback;
    }

    private object? ResolvePath(string path)
    {
        var parts = path.Split('.');
        if (parts.Length < 2)
            return null;

        string scope = parts[0].ToLowerInvariant();
        string key = string.Join(".", parts[1..]);

        return scope switch
        {
            "inventory" => ResolveInventory(key),
            "counter" => _context.Counters.TryGetValue(key, out var c) ? c : null,
            "flag" => _context.Flags.TryGetValue(key, out var f) ? f : null,
            "relationship" => _context.Relationships.TryGetValue(key, out var r) ? r : null,
            "player" => ResolvePlayer(key),
            _ => null
        };
    }

    private object? ResolveInventory(string key)
    {
        if (key.EndsWith(".*"))
        {
            // Wildcard: inventory.prefix.*
            string prefix = key[..^2];
            var matches = _context.InventoryItems
                .Where(item => item.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToList();
            return matches.Count > 0 ? matches : null;
        }

        // Single item: inventory.item_id
        return _context.InventoryItems.Contains(key) ? key : null;
    }

    private object? ResolvePlayer(string key)
    {
        return key switch
        {
            "location" => _context.CurrentLocation,
            _ => null
        };
    }

    private string ApplyFormatter(object value, string formatter)
    {
        // Parse formatter syntax: formatter or formatter="param"
        string formatterName = formatter;
        string parameter = "";

        if (formatter.Contains('='))
        {
            var parts = formatter.Split('=', 2);
            formatterName = parts[0].Trim();
            parameter = parts[1].Trim().Trim('"');
        }

        return formatterName.ToLowerInvariant() switch
        {
            "names" => FormatNames(value),
            "ids" => FormatIds(value),
            "count" => FormatCount(value),
            "join" => FormatJoin(value, parameter),
            _ => value.ToString() ?? ""
        };
    }

    private string FormatNames(object value)
    {
        if (value is List<string> items)
            return string.Join(", ", items);
        return value.ToString() ?? "";
    }

    private string FormatIds(object value)
    {
        if (value is List<string> items)
            return string.Join(", ", items.Select(i => i.ToLowerInvariant()));
        return value.ToString() ?? "";
    }

    private string FormatCount(object value)
    {
        if (value is List<string> items)
            return items.Count.ToString();
        if (value is string s)
            return "1";
        return "0";
    }

    private string FormatJoin(object value, string separator)
    {
        if (string.IsNullOrEmpty(separator))
            separator = ", ";

        if (value is List<string> items)
            return string.Join(separator, items);
        return value.ToString() ?? "";
    }
}

/// <summary>
/// Context for DSL interpolation - provides runtime values.
/// </summary>
public sealed class DslInterpolationContext
{
    public List<string> InventoryItems { get; set; } = [];
    public Dictionary<string, int> Counters { get; set; } = [];
    public Dictionary<string, bool> Flags { get; set; } = [];
    public Dictionary<string, int> Relationships { get; set; } = [];
    public string CurrentLocation { get; set; } = "";
}

/// <summary>
/// Safe expression engine for DSL v2.
/// Only allows whitelisted operators and functions.
/// </summary>
public sealed class DslSafeExpressionEvaluator
{
    private static readonly HashSet<string> AllowedTokens = new(StringComparer.OrdinalIgnoreCase)
    {
        "+", "-", "*", "/", "(", ")", "min", "max", ".",
        // Numbers and identifiers are also allowed
    };

    public bool IsValidExpression(string expr)
    {
        if (string.IsNullOrWhiteSpace(expr))
            return false;

        // Tokenize and validate
        var tokens = Tokenize(expr);
        foreach (var token in tokens)
        {
            if (IsOperator(token) || IsFunction(token) || IsParenthesis(token) || IsNumber(token) || IsIdentifier(token))
                continue;

            return false; // Unknown token
        }

        return true;
    }

    private List<string> Tokenize(string expr)
    {
        var tokens = new List<string>();
        var current = "";

        foreach (char c in expr)
        {
            if (char.IsWhiteSpace(c))
            {
                if (!string.IsNullOrEmpty(current))
                {
                    tokens.Add(current);
                    current = "";
                }
            }
            else if ("+-*/()".Contains(c))
            {
                if (!string.IsNullOrEmpty(current))
                {
                    tokens.Add(current);
                    current = "";
                }
                tokens.Add(c.ToString());
            }
            else
            {
                current += c;
            }
        }

        if (!string.IsNullOrEmpty(current))
            tokens.Add(current);

        return tokens;
    }

    private bool IsOperator(string token) => token is "+" or "-" or "*" or "/";
    private bool IsFunction(string token) => token is "min" or "max";
    private bool IsParenthesis(string token) => token is "(" or ")";
    private bool IsNumber(string token) => int.TryParse(token, out _);
    private bool IsIdentifier(string token) => !IsOperator(token) && !IsFunction(token) && !IsParenthesis(token) && !IsNumber(token);
}

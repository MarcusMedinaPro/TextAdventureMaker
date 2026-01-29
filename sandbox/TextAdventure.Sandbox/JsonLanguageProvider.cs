// <copyright file="JsonLanguageProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace TextAdventure.Sandbox;

using System.Globalization;
using System.Text.Json;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Localization;

public sealed class JsonLanguageProvider : ILanguageProvider
{
    private readonly JsonLanguageData _data;

    public JsonLanguageProvider(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var json = File.ReadAllText(path);
        _data = JsonSerializer.Deserialize<JsonLanguageData>(json, JsonOptions)
            ?? throw new InvalidOperationException($"Failed to parse language file: {path}");
    }

    private static JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    public string Code => _data.Meta?.Code ?? "en";
    public string Name => _data.Meta?.Name ?? "English";

    public string Get(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return "";

        // Normalize key: remove "Template" suffix and convert to camelCase
        var normalizedKey = NormalizeKey(key);

        // Try labels first
        if (_data.Labels?.TryGetValue(normalizedKey, out var label) == true)
            return label;

        // Then messages
        if (_data.Messages?.TryGetValue(normalizedKey, out var message) == true)
            return message;

        // Then templates
        if (_data.Templates?.TryGetValue(normalizedKey, out var template) == true)
            return template;

        return $"[[{key}]]";
    }

    private static string NormalizeKey(string key)
    {
        // Remove "Template" suffix if present
        if (key.EndsWith("Template", StringComparison.Ordinal))
            key = key[..^8];

        // Convert PascalCase to camelCase
        if (key.Length > 0 && char.IsUpper(key[0]))
            return char.ToLowerInvariant(key[0]) + key[1..];

        return key;
    }

    public string Format(string key, params object[] args)
    {
        var template = Get(key);
        return string.Format(CultureInfo.InvariantCulture, template, args);
    }

    public string GetName(string id) =>
        _data.Names?.TryGetValue(id, out var name) == true ? name : id;

    public string GetDescription(string id) =>
        _data.Descriptions?.TryGetValue(id, out var desc) == true ? desc : "";

    public string GetDirectionName(Direction direction)
    {
        var key = direction.ToString().ToLowerInvariant();
        return _data.Directions?.TryGetValue(key, out var dir) == true ? dir.Name : direction.ToString();
    }

    public IReadOnlyList<string> GetCommandAliases(string command) =>
        _data.Commands?.TryGetValue(command, out var aliases) == true ? aliases : [];

    public IReadOnlyDictionary<string, Direction> GetDirectionAliases()
    {
        var result = new Dictionary<string, Direction>(StringComparer.OrdinalIgnoreCase);
        if (_data.Directions == null)
            return result;

        foreach (var (key, dir) in _data.Directions)
        {
            if (!Enum.TryParse<Direction>(key, true, out var direction))
                continue;

            foreach (var alias in dir.Aliases)
            {
                result[alias] = direction;
            }
        }

        return result;
    }

    public ISet<string> GetAllCommandAliases(string command)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (_data.Commands?.TryGetValue(command, out var aliases) == true)
        {
            foreach (var alias in aliases)
                set.Add(alias);
        }
        return set;
    }
}

public sealed class JsonLanguageData
{
    public LanguageMeta? Meta { get; set; }
    public Dictionary<string, List<string>>? Commands { get; set; }
    public Dictionary<string, DirectionData>? Directions { get; set; }
    public Dictionary<string, string>? Labels { get; set; }
    public Dictionary<string, string>? Messages { get; set; }
    public Dictionary<string, string>? Templates { get; set; }
    public Dictionary<string, string>? Names { get; set; }
    public Dictionary<string, string>? Descriptions { get; set; }
}

public sealed class LanguageMeta
{
    public string? Code { get; set; }
    public string? Name { get; set; }
}

public sealed class DirectionData
{
    public string Name { get; set; } = "";
    public List<string> Aliases { get; set; } = [];
}

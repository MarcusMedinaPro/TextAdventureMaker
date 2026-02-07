// <copyright file="JsonLanguageProvider.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Globalization;
using System.Text.Json;
using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Localization;

public sealed class JsonLanguageProvider : ILanguageProvider
{
    private readonly JsonLanguageData _data;

    public JsonLanguageProvider(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        string json = File.ReadAllText(path);
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
        {
            return "";
        }

        string normalizedKey = NormalizeKey(key);

        if (_data.Labels?.TryGetValue(normalizedKey, out string? label) == true)
        {
            return label;
        }

        if (_data.Messages?.TryGetValue(normalizedKey, out string? message) == true)
        {
            return message;
        }

        return _data.Templates?.TryGetValue(normalizedKey, out string? template) == true ? template : $"[[{key}]]";
    }

    private static string NormalizeKey(string key)
    {
        if (key.EndsWith("Template", StringComparison.Ordinal))
        {
            key = key[..^8];
        }

        return key.Length > 0 && char.IsUpper(key[0]) ? char.ToLowerInvariant(key[0]) + key[1..] : key;
    }

    public string Format(string key, params object[] args)
    {
        string template = Get(key);
        return string.Format(CultureInfo.InvariantCulture, template, args);
    }

    public string GetName(string id)
    {
        return _data.Names?.TryGetValue(id, out string? name) == true ? name : id;
    }

    public string GetDescription(string id)
    {
        return _data.Descriptions?.TryGetValue(id, out string? desc) == true ? desc : "";
    }

    public string GetDirectionName(Direction direction)
    {
        string key = direction.ToString().ToLowerInvariant();
        return _data.Directions?.TryGetValue(key, out DirectionData? dir) == true ? dir.Name : direction.ToString();
    }

    public IReadOnlyList<string> GetCommandAliases(string command)
    {
        return _data.Commands?.TryGetValue(command, out List<string>? aliases) == true ? aliases : [];
    }

    public IReadOnlyDictionary<string, Direction> GetDirectionAliases()
    {
        Dictionary<string, Direction> result = new(StringComparer.OrdinalIgnoreCase);
        if (_data.Directions == null)
        {
            return result;
        }

        foreach ((string? key, DirectionData? dir) in _data.Directions)
        {
            if (!Enum.TryParse<Direction>(key, true, out Direction direction))
            {
                continue;
            }

            foreach (string alias in dir.Aliases)
            {
                result[alias] = direction;
            }
        }

        return result;
    }

    public ISet<string> GetAllCommandAliases(string command)
    {
        HashSet<string> set = new(StringComparer.OrdinalIgnoreCase);
        if (_data.Commands?.TryGetValue(command, out List<string>? aliases) == true)
        {
            foreach (string alias in aliases)
            {
                _ = set.Add(alias);
            }
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

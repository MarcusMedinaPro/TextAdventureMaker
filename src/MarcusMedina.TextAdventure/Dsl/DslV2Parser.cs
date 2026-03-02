// <copyright file="DslV2Parser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 parser - extended keywords for entities, items, and start-state definitions.
/// Extends AdventureDslParser with new v2 features without breaking v1 compatibility.
/// </summary>
public sealed class DslV2Parser : AdventureDslParser
{
    private readonly Dictionary<string, DslEntityDefinition> _definedItems = [];
    private readonly Dictionary<string, DslEntityDefinition> _definedNpcs = [];
    private readonly DslStartStateDefinition _startState = new();

    public DslV2Parser()
    {
        RegisterV2Keywords();
    }

    private void RegisterV2Keywords()
    {
        // Entity definition keywords
        RegisterKeyword("define item", HandleDefineItem);
        RegisterKeyword("define key", HandleDefineKey);
        RegisterKeyword("define door", HandleDefineDoor);
        RegisterKeyword("define npc", HandleDefineNpc);

        // Placement keywords
        RegisterKeyword("place item", HandlePlaceItem);
        RegisterKeyword("place npc", HandlePlaceNpc);

        // Start-state keywords
        RegisterKeyword("current_location", HandleCurrentLocation);
        RegisterKeyword("start_inventory", HandleStartInventory);
        RegisterKeyword("start_stats", HandleStartStats);
        RegisterKeyword("flag", HandleFlag);
        RegisterKeyword("counter", HandleCounter);
        RegisterKeyword("relationship", HandleRelationship);
        RegisterKeyword("timeline", HandleTimeline);
    }

    private void HandleDefineItem(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count == 0) return;

        string id = NormalizeId(parts[0]);
        var definition = ParseEntityDefinition(value);
        _definedItems[id] = definition;
    }

    private void HandleDefineKey(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count == 0) return;

        string id = NormalizeId(parts[0]);
        var definition = ParseEntityDefinition(value);
        definition.IsKey = true;
        _definedItems[id] = definition;
    }

    private void HandleDefineDoor(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count == 0) return;

        string id = NormalizeId(parts[0]);
        var definition = ParseEntityDefinition(value);
        definition.IsDoor = true;
        _definedItems[id] = definition;
    }

    private void HandleDefineNpc(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count == 0) return;

        string id = NormalizeId(parts[0]);
        var definition = ParseEntityDefinition(value);
        _definedNpcs[id] = definition;
    }

    private void HandlePlaceItem(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string locationId = NormalizeId(parts[0]);
        string itemId = NormalizeId(parts[1]);

        if (!_definedItems.TryGetValue(itemId, out var definition))
        {
            context.AddWarning(new DslParseError(context.CurrentLineNumber, "Item not found", $"Item '{itemId}' not defined before placement"));
            return;
        }

        // Find location and add item
        // This would be implemented based on how contexts store locations
    }

    private void HandlePlaceNpc(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string locationId = NormalizeId(parts[0]);
        string npcId = NormalizeId(parts[1]);

        if (!_definedNpcs.TryGetValue(npcId, out var definition))
        {
            context.AddWarning(new DslParseError(context.CurrentLineNumber, "NPC not found", $"NPC '{npcId}' not defined before placement"));
            return;
        }

        // Find location and add NPC
        // This would be implemented based on how contexts store locations
    }

    private void HandleCurrentLocation(AdventureDslContext context, string value)
    {
        _startState.CurrentLocationId = NormalizeId(value);
    }

    private void HandleStartInventory(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count == 0) return;

        string itemId = NormalizeId(parts[0]);
        var inventoryEntry = new DslInventoryEntry { ItemId = itemId };

        // Parse options like amount=3
        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].Contains("="))
            {
                var kv = parts[i].Split('=');
                if (kv.Length == 2 && kv[0].Equals("amount", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(kv[1], out var amount))
                        inventoryEntry.Amount = amount;
                }
            }
        }

        _startState.StartInventory.Add(inventoryEntry);
    }

    private void HandleStartStats(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        foreach (var part in parts)
        {
            if (part.Contains("="))
            {
                var kv = part.Split('=');
                if (kv.Length == 2)
                {
                    _startState.StartStats[kv[0]] = kv[1];
                }
            }
        }
    }

    private void HandleFlag(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string flagDef = parts[0];
        if (flagDef.Contains("="))
        {
            var kv = flagDef.Split('=');
            _startState.Flags[kv[0]] = kv[1].Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }

    private void HandleCounter(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string counterDef = parts[0];
        if (counterDef.Contains("="))
        {
            var kv = counterDef.Split('=');
            if (int.TryParse(kv[1], out var count))
                _startState.Counters[kv[0]] = count;
        }
    }

    private void HandleRelationship(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string relDef = parts[0];
        if (relDef.Contains("="))
        {
            var kv = relDef.Split('=');
            if (int.TryParse(kv[1], out var score))
                _startState.Relationships[kv[0]] = score;
        }
    }

    private void HandleTimeline(AdventureDslContext context, string value)
    {
        _startState.TimelineId = NormalizeId(value);
    }

    private DslEntityDefinition ParseEntityDefinition(string value)
    {
        var parts = SplitParts(value);
        var definition = new DslEntityDefinition
        {
            Id = parts.Count > 0 ? parts[0] : "",
            Name = parts.Count > 1 ? parts[1] : "",
            Description = parts.Count > 2 ? parts[2] : "",
            Options = new()
        };

        // Parse key=value options
        for (int i = 3; i < parts.Count; i++)
        {
            if (parts[i].Contains("="))
            {
                var kv = parts[i].Split('=', 2);
                definition.Options[kv[0]] = kv.Length > 1 ? kv[1] : "";
            }
        }

        return definition;
    }

    private static List<string> SplitParts(string input)
    {
        // Simple pipe-based splitting - can be enhanced later
        return input.Split('|').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToList();
    }

    private static string NormalizeId(string input)
    {
        return input.Trim().ToLowerInvariant().Replace(" ", "_");
    }

    public DslStartStateDefinition GetStartState() => _startState;
    public IReadOnlyDictionary<string, DslEntityDefinition> GetDefinedItems() => _definedItems;
    public IReadOnlyDictionary<string, DslEntityDefinition> GetDefinedNpcs() => _definedNpcs;
}

/// <summary>
/// Entity definition for DSL v2.
/// </summary>
public sealed record DslEntityDefinition
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public Dictionary<string, string> Options { get; set; } = [];
    public bool IsKey { get; set; }
    public bool IsDoor { get; set; }
}

/// <summary>
/// Start-state definition for DSL v2.
/// </summary>
public sealed class DslStartStateDefinition
{
    public string CurrentLocationId { get; set; } = "";
    public List<DslInventoryEntry> StartInventory { get; set; } = [];
    public Dictionary<string, string> StartStats { get; set; } = [];
    public Dictionary<string, bool> Flags { get; set; } = [];
    public Dictionary<string, int> Counters { get; set; } = [];
    public Dictionary<string, int> Relationships { get; set; } = [];
    public string TimelineId { get; set; } = "";
}

/// <summary>
/// Inventory entry for start-state.
/// </summary>
public sealed class DslInventoryEntry
{
    public string ItemId { get; set; } = "";
    public int Amount { get; set; } = 1;
}

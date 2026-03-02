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
    private readonly List<DslItemReaction> _itemReactions = [];
    private readonly List<DslItemConsequence> _itemConsequences = [];
    private readonly List<DslRecipe> _recipes = [];
    private readonly List<DslDoorConfig> _doorConfigs = [];
    private readonly List<DslExitConfig> _exitConfigs = [];
    private readonly List<DslRoomDescription> _roomDescriptions = [];
    private readonly List<DslRoomDescriptionCondition> _roomDescConditions = [];
    private readonly List<DslRoomVariable> _roomVariables = [];
    private readonly List<DslRoomTransform> _roomTransforms = [];

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

        // Item reactions and consequences (Slice 074)
        RegisterKeyword("item_reaction", HandleItemReaction);
        RegisterKeyword("item_consequence", HandleItemConsequence);
        RegisterKeyword("recipe", HandleRecipe);

        // World interaction (Slice 076)
        RegisterKeyword("door_config", HandleDoorConfig);
        RegisterKeyword("exit_config", HandleExitConfig);
        RegisterKeyword("room_desc", HandleRoomDesc);
        RegisterKeyword("room_desc_when", HandleRoomDescWhen);
        RegisterKeyword("room_var", HandleRoomVar);
        RegisterKeyword("room_transform", HandleRoomTransform);
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

    private void HandleItemReaction(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string itemId = NormalizeId(parts[0]);
        string? action = null;
        string? text = null;

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("on="))
                action = parts[i][3..];
            else if (parts[i].StartsWith("text="))
                text = parts[i][5..];
        }

        if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(text))
        {
            _itemReactions.Add(new DslItemReaction { ItemId = itemId, Action = action, Text = text });
        }
    }

    private void HandleItemConsequence(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string itemId = NormalizeId(parts[0]);
        string? action = null;
        var consequence = new DslItemConsequence { ItemId = itemId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("on="))
                action = parts[i][3..];
            else if (parts[i].StartsWith("destroy="))
                consequence.Destroy = parts[i][8..].Equals("true", StringComparison.OrdinalIgnoreCase);
            else if (parts[i].StartsWith("create="))
                consequence.Create = parts[i][7..].Split(',').Select(x => NormalizeId(x.Trim())).ToList();
            else if (parts[i].StartsWith("transform="))
                consequence.Transform = NormalizeId(parts[i][10..]);
            else if (parts[i].StartsWith("set_flag="))
            {
                var kv = parts[i][9..].Split(':');
                if (kv.Length == 2)
                    consequence.SetFlags[kv[0]] = kv[1].Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            else if (parts[i].StartsWith("inc_counter="))
            {
                var kv = parts[i][12..].Split(':');
                if (kv.Length == 2 && int.TryParse(kv[1], out var val))
                    consequence.IncrementCounters[kv[0]] = val;
            }
            else if (parts[i].StartsWith("message="))
                consequence.Message = parts[i][8..];
        }

        if (!string.IsNullOrEmpty(action))
        {
            consequence.Action = action;
            _itemConsequences.Add(consequence);
        }
    }

    private void HandleRecipe(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string recipeId = NormalizeId(parts[0]);
        var recipe = new DslRecipe { Id = recipeId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("left="))
                recipe.LeftItemId = NormalizeId(parts[i][5..]);
            else if (parts[i].StartsWith("right="))
                recipe.RightItemId = NormalizeId(parts[i][6..]);
            else if (parts[i].StartsWith("create="))
                recipe.CreatedItemId = NormalizeId(parts[i][7..]);
            else if (parts[i].StartsWith("message="))
                recipe.Message = parts[i][8..];
        }

        if (!string.IsNullOrEmpty(recipe.LeftItemId) && !string.IsNullOrEmpty(recipe.RightItemId) && !string.IsNullOrEmpty(recipe.CreatedItemId))
        {
            _recipes.Add(recipe);
        }
    }

    private void HandleDoorConfig(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string doorId = NormalizeId(parts[0]);
        var config = new DslDoorConfig { Id = doorId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("state="))
                config.State = parts[i][6..];
            else if (parts[i].StartsWith("aliases="))
                config.Aliases = parts[i][8..].Split(',').Select(x => x.Trim()).ToList();
            else if (parts[i].StartsWith("reaction."))
            {
                var kv = parts[i][9..].Split('=');
                if (kv.Length == 2)
                    config.Reactions[kv[0]] = kv[1];
            }
        }

        _doorConfigs.Add(config);
    }

    private void HandleExitConfig(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string fromId = NormalizeId(parts[0]);
        string toId = NormalizeId(parts[1]);
        var config = new DslExitConfig { FromLocationId = fromId, ToLocationId = toId };

        for (int i = 2; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("direction="))
                config.Direction = parts[i][10..];
            else if (parts[i].Equals("hidden=true", StringComparison.OrdinalIgnoreCase))
                config.Hidden = true;
            else if (parts[i].StartsWith("discover_if="))
                config.DiscoverIf = parts[i][12..];
            else if (parts[i].StartsWith("perception="))
            {
                if (int.TryParse(parts[i][11..], out var p))
                    config.Perception = Math.Clamp(p, 1, 100);
            }
            else if (parts[i].StartsWith("door="))
                config.DoorId = NormalizeId(parts[i][5..]);
            else if (parts[i].Equals("oneway=true", StringComparison.OrdinalIgnoreCase))
                config.OneWay = true;
        }

        _exitConfigs.Add(config);
    }

    private void HandleRoomDesc(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string locationId = NormalizeId(parts[0]);
        var desc = new DslRoomDescription { LocationId = locationId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("default="))
                desc.DefaultDescription = parts[i][8..];
            else if (parts[i].StartsWith("first_visit="))
                desc.FirstVisitDescription = parts[i][12..];
        }

        _roomDescriptions.Add(desc);
    }

    private void HandleRoomDescWhen(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string locationId = NormalizeId(parts[0]);
        var condition = new DslRoomDescriptionCondition { LocationId = locationId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("if="))
                condition.Condition = parts[i][3..];
            else if (parts[i].StartsWith("text="))
                condition.Text = parts[i][5..];
        }

        if (!string.IsNullOrEmpty(condition.Condition) && !string.IsNullOrEmpty(condition.Text))
            _roomDescConditions.Add(condition);
    }

    private void HandleRoomVar(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string locationId = NormalizeId(parts[0]);
        var roomVar = new DslRoomVariable { LocationId = locationId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("key="))
                roomVar.Key = parts[i][4..];
            else if (parts[i].StartsWith("value="))
                roomVar.Value = parts[i][6..];
        }

        if (!string.IsNullOrEmpty(roomVar.Key) && !string.IsNullOrEmpty(roomVar.Value))
            _roomVariables.Add(roomVar);
    }

    private void HandleRoomTransform(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string locationId = NormalizeId(parts[0]);
        var transform = new DslRoomTransform { SourceLocationId = locationId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("target="))
                transform.TargetLocationId = NormalizeId(parts[i][7..]);
            else if (parts[i].StartsWith("if="))
                transform.Condition = parts[i][3..];
            else if (parts[i].Equals("irreversible=true", StringComparison.OrdinalIgnoreCase))
                transform.Irreversible = true;
        }

        if (!string.IsNullOrEmpty(transform.TargetLocationId))
            _roomTransforms.Add(transform);
    }

    public DslStartStateDefinition GetStartState() => _startState;
    public IReadOnlyDictionary<string, DslEntityDefinition> GetDefinedItems() => _definedItems;
    public IReadOnlyDictionary<string, DslEntityDefinition> GetDefinedNpcs() => _definedNpcs;
    public IReadOnlyList<DslItemReaction> GetItemReactions() => _itemReactions;
    public IReadOnlyList<DslItemConsequence> GetItemConsequences() => _itemConsequences;
    public IReadOnlyList<DslRecipe> GetRecipes() => _recipes;
    public IReadOnlyList<DslDoorConfig> GetDoorConfigs() => _doorConfigs;
    public IReadOnlyList<DslExitConfig> GetExitConfigs() => _exitConfigs;
    public IReadOnlyList<DslRoomDescription> GetRoomDescriptions() => _roomDescriptions;
    public IReadOnlyList<DslRoomDescriptionCondition> GetRoomDescConditions() => _roomDescConditions;
    public IReadOnlyList<DslRoomVariable> GetRoomVariables() => _roomVariables;
    public IReadOnlyList<DslRoomTransform> GetRoomTransforms() => _roomTransforms;
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

/// <summary>
/// Item reaction definition for DSL v2.
/// </summary>
public sealed class DslItemReaction
{
    public string ItemId { get; set; } = "";
    public string Action { get; set; } = "";
    public string Text { get; set; } = "";
}

/// <summary>
/// Item consequence definition for DSL v2.
/// </summary>
public sealed class DslItemConsequence
{
    public string ItemId { get; set; } = "";
    public string Action { get; set; } = "";
    public bool Destroy { get; set; }
    public List<string> Create { get; set; } = [];
    public string Transform { get; set; } = "";
    public Dictionary<string, bool> SetFlags { get; set; } = [];
    public Dictionary<string, int> IncrementCounters { get; set; } = [];
    public string Message { get; set; } = "";
}

/// <summary>
/// Recipe definition for DSL v2 (item combinations).
/// </summary>
public sealed class DslRecipe
{
    public string Id { get; set; } = "";
    public string LeftItemId { get; set; } = "";
    public string RightItemId { get; set; } = "";
    public string CreatedItemId { get; set; } = "";
    public string Message { get; set; } = "";
}

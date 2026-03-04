// <copyright file="DslParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 parser - extended keywords for entities, items, and start-state definitions.
/// Extends AdventureDslParser with new v2 features without breaking v1 compatibility.
/// </summary>
public sealed class DslParser : AdventureDslParser
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
    private readonly List<DslNpcDef> _npcDefs = [];
    private readonly List<DslNpcPlacement> _npcPlacements = [];
    private readonly List<DslNpcDialog> _npcDialogs = [];
    private readonly List<DslNpcAcceptanceRule> _acceptanceRules = [];
    private readonly List<DslNpcAcceptanceDefault> _acceptanceDefaults = [];
    private int _acceptanceRulePriority = 0;
    private readonly List<DslNpcDialogOption> _dialogOptions = [];
    private readonly List<DslNpcRule> _npcRules = [];
    private readonly List<DslNpcTrigger> _npcTriggers = [];
    private int _npcRulePriority = 1000; // Higher = evaluated first
    private readonly List<DslQuest> _quests = [];
    private readonly List<DslQuestStage> _questStages = [];
    private readonly List<DslQuestObjective> _questObjectives = [];
    private readonly List<DslQuestCondition> _questConditions = [];
    private readonly List<DslQuestOnComplete> _questOnCompletes = [];
    private readonly List<DslQuestOnFail> _questOnFails = [];
    private readonly List<DslTrigger> _triggers = [];
    private readonly List<DslSchedule> _schedules = [];
    private DslRandomSettings _randomSettings = new();
    private readonly List<DslRandomEvent> _randomEvents = [];
    private int _scheduleId = 0;
    private readonly DslStorySystem _storySystem = new();
    private string _currentBranchId = "";
    private string _currentChapterId = "";
    private readonly List<DslNpcIdleBehavior> _npcIdleBehaviors = [];
    private readonly DslParserConfiguration _parserConfig = new();

    public DslParser()
    {
        RegisterV2Keywords();
    }

    protected override void OnAfterParse(AdventureDslContext context)
    {
        base.OnAfterParse(context);
        ApplyItemRuntimeData(context);
        ApplyNpcRuntimeData(context);
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

        // NPC definitions (Slice 077)
        RegisterKeyword("npc", HandleNpc);
        RegisterKeyword("npc_place", HandleNpcPlace);
        RegisterKeyword("npc_dialog", HandleNpcDialog);
        RegisterKeyword("npc_acceptance", HandleNpcAcceptance);
        RegisterKeyword("npc_acceptance_default", HandleNpcAcceptanceDefault);

        // NPC rules and triggers (Slice 078)
        RegisterKeyword("npc_dialog_option", HandleNpcDialogOption);
        RegisterKeyword("npc_rule", HandleNpcRule);
        RegisterKeyword("npc_trigger", HandleNpcTrigger);

        // Quest definitions (Slice 079)
        RegisterKeyword("quest", HandleQuest);
        RegisterKeyword("quest_stage", HandleQuestStage);
        RegisterKeyword("quest_objective", HandleQuestObjective);
        RegisterKeyword("quest_condition", HandleQuestCondition);
        RegisterKeyword("quest_on_complete", HandleQuestOnComplete);
        RegisterKeyword("quest_on_fail", HandleQuestOnFail);

        // Event and schedule (Slice 080)
        RegisterKeyword("on_enter", (ctx, val) => HandleTrigger(ctx, val, "on_enter"));
        RegisterKeyword("on_pickup", (ctx, val) => HandleTrigger(ctx, val, "on_pickup"));
        RegisterKeyword("on_talk", (ctx, val) => HandleTrigger(ctx, val, "on_talk"));
        RegisterKeyword("on_action", (ctx, val) => HandleTrigger(ctx, val, "on_action"));
        RegisterKeyword("on_npc_death", (ctx, val) => HandleTrigger(ctx, val, "on_npc_death"));
        RegisterKeyword("on_tick", (ctx, val) => HandleTrigger(ctx, val, "on_tick"));
        RegisterKeyword("schedule_at", HandleScheduleAt);
        RegisterKeyword("schedule_every", HandleScheduleEvery);
        RegisterKeyword("schedule_when", HandleScheduleWhen);
        RegisterKeyword("random_settings", HandleRandomSettings);
        RegisterKeyword("random_event", HandleRandomEvent);

        // Story and chapters (Slice 083)
        RegisterKeyword("branch", HandleBranch);
        RegisterKeyword("branch_when", HandleBranchWhen);
        RegisterKeyword("branch_then", HandleBranchThen);
        RegisterKeyword("chapter", HandleChapter);
        RegisterKeyword("chapter_objective", HandleChapterObjective);
        RegisterKeyword("chapter_next", HandleChapterNext);
        RegisterKeyword("chapter_end", HandleChapterEnd);
        RegisterKeyword("chapter_unlock_if", HandleChapterUnlockIf);

        // Parser configuration (Slice 084)
        RegisterKeyword("parser_option", HandleParserOption);
        RegisterKeyword("command_alias", HandleCommandAlias);
        RegisterKeyword("direction_alias", HandleDirectionAlias);

        // Custom verbs and NPC reactions (Slice 094)
        RegisterKeyword("command", HandleCustomCommand);
        RegisterKeyword("npc_reaction", HandleNpcReaction);

        // NPC idle behaviour
        RegisterKeyword("npc_idle", HandleNpcIdle);
    }

    private void ApplyItemRuntimeData(AdventureDslContext context)
    {
        if (_itemReactions.Count == 0)
            return;

        foreach (DslItemReaction reaction in _itemReactions)
        {
            string itemId = NormalizeId(reaction.ItemId);
            if (string.IsNullOrWhiteSpace(itemId))
                continue;

            if (!TryResolveItem(context, itemId, out Item item))
            {
                context.AddWarning(new DslParseError(
                    0,
                    $"item_reaction: {reaction.ItemId}",
                    $"Item '{itemId}' referenced by item_reaction is not defined"));
                continue;
            }

            if (!TryParseItemAction(reaction.Action, out ItemAction action))
            {
                context.AddWarning(new DslParseError(
                    0,
                    $"item_reaction: {reaction.ItemId} | on={reaction.Action}",
                    $"Unsupported item action '{reaction.Action}' in item_reaction"));
                continue;
            }

            if (!string.IsNullOrWhiteSpace(reaction.Text))
                _ = item.SetReaction(action, reaction.Text);
        }
    }

    private static bool TryResolveItem(AdventureDslContext context, string itemId, out Item item)
    {
        if (context.Items.TryGetValue(itemId, out Item? foundItem))
        {
            item = foundItem;
            return true;
        }

        if (context.Keys.TryGetValue(itemId, out Key? foundKey))
        {
            item = foundKey;
            return true;
        }

        item = null!;
        return false;
    }

    private static bool TryParseItemAction(string actionText, out ItemAction action)
    {
        if (string.IsNullOrWhiteSpace(actionText))
        {
            action = default;
            return false;
        }

        string normalized = actionText
            .Trim()
            .Replace("_", "", StringComparison.Ordinal)
            .Replace("-", "", StringComparison.Ordinal)
            .Replace(" ", "", StringComparison.Ordinal);

        return Enum.TryParse(normalized, true, out action);
    }

    private void ApplyNpcRuntimeData(AdventureDslContext context)
    {
        if (_npcDefs.Count == 0)
            return;

        Dictionary<string, Npc> npcs = new(StringComparer.OrdinalIgnoreCase);

        foreach (DslNpcDef definition in _npcDefs)
        {
            string id = NormalizeId(definition.Id);
            if (string.IsNullOrWhiteSpace(id))
                continue;

            string name = string.IsNullOrWhiteSpace(definition.Name)
                ? id.Replace('_', ' ')
                : definition.Name.Trim();

            var npc = new Npc(id, name, ParseNpcState(definition.State), new Stats(Math.Max(1, definition.Health)));

            if (!string.IsNullOrWhiteSpace(definition.Description))
                npc.Description(definition.Description);

            if (TryParseArchetype(definition.Archetype, out CharacterArchetype archetype))
                npc.SetArchetype(archetype);

            if (TryParseJourneyStage(definition.DiesAt, out JourneyStage stage))
                npc.DiesAt(stage);

            npc.SetMovement(ParseNpcMovement(definition.Movement, context));
            npcs[id] = npc;
        }

        foreach (DslNpcDialog dialog in _npcDialogs)
        {
            string npcId = NormalizeId(dialog.NpcId);
            if (!npcs.TryGetValue(npcId, out Npc? npc))
            {
                context.AddWarning(new DslParseError(
                    0,
                    $"npc_dialog: {dialog.NpcId}",
                    $"NPC '{npcId}' referenced by npc_dialog is not defined"));
                continue;
            }

            if (!string.IsNullOrWhiteSpace(dialog.Text))
                npc.Dialog(dialog.Text);
        }

        foreach (DslNpcPlacement placement in _npcPlacements)
        {
            string locationId = NormalizeId(placement.LocationId);
            string npcId = NormalizeId(placement.NpcId);

            if (!npcs.TryGetValue(npcId, out Npc? npc))
            {
                context.AddWarning(new DslParseError(
                    0,
                    $"npc_place: {placement.LocationId} | {placement.NpcId}",
                    $"NPC '{npcId}' referenced by npc_place is not defined"));
                continue;
            }

            if (!context.Locations.TryGetValue(locationId, out Location? location))
            {
                context.AddWarning(new DslParseError(
                    0,
                    $"npc_place: {placement.LocationId} | {placement.NpcId}",
                    $"Location '{locationId}' referenced by npc_place is not defined"));
                continue;
            }

            if (location.FindNpc(npc.Id) is null)
                location.AddNpc(npc);
        }

        foreach (DslNpcReaction reaction in _parserConfig.NpcReactions)
        {
            if (!npcs.TryGetValue(reaction.NpcId, out Npc? npc))
            {
                context.AddWarning(new DslParseError(
                    0,
                    $"npc_reaction: {reaction.NpcId}",
                    $"NPC '{reaction.NpcId}' referenced by npc_reaction is not defined"));
                continue;
            }

            Func<IGameState, bool>? condition = ParseNpcReactionCondition(reaction.Condition);
            Action<IGameState>? effect = BuildNpcReactionEffect(reaction);
            npc.AddReaction(reaction.Trigger, reaction.Text, condition, reaction.EndGame, effect);
        }

        foreach (DslNpcIdleBehavior idle in _npcIdleBehaviors)
        {
            if (!npcs.TryGetValue(idle.NpcId, out Npc? npc))
            {
                context.AddWarning(new DslParseError(
                    0,
                    $"npc_idle: {idle.NpcId}",
                    $"NPC '{idle.NpcId}' referenced by npc_idle is not defined"));
                continue;
            }

            npc.AddIdleBehavior(idle.Interval, [.. idle.Messages]);
        }

        if (_dialogOptions.Count > 0)
        {
            context.AddWarning(new DslParseError(
                0,
                "npc_dialog_option",
                "npc_dialog_option is parsed but not yet bound into runtime dialog trees"));
        }
    }

    private static Func<IGameState, bool>? ParseNpcReactionCondition(string? condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
            return null;

        // Multiple conditions joined by commas — all must pass
        string[] clauses = condition.Split(',').Select(c => c.Trim()).Where(c => c.Length > 0).ToArray();
        var funcs = clauses.Select(ParseSingleCondition).Where(f => f is not null).ToList();

        return funcs.Count switch
        {
            0 => null,
            1 => funcs[0],
            _ => state => funcs.All(f => f!(state))
        };
    }

    private static Func<IGameState, bool>? ParseSingleCondition(string clause)
    {
        // flag:key=value
        if (clause.StartsWith("flag:", StringComparison.OrdinalIgnoreCase))
        {
            string[] kv = clause[5..].Split('=');
            if (kv.Length == 2)
            {
                string key = kv[0].Trim();
                bool expected = kv[1].Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
                return state => state.WorldState.GetFlag(key) == expected;
            }
            return null;
        }

        // has_item=item_id
        if (clause.StartsWith("has_item=", StringComparison.OrdinalIgnoreCase))
        {
            string id = clause[9..].Trim();
            return state => state.Inventory.FindById(id) is not null;
        }

        // door_unlocked=door_id
        if (clause.StartsWith("door_unlocked=", StringComparison.OrdinalIgnoreCase))
        {
            string doorId = clause[14..].Trim();
            return state => state.Locations
                .SelectMany(loc => loc.Exits.Values)
                .Any(exit => exit.Door?.Id.Equals(doorId, StringComparison.OrdinalIgnoreCase) == true
                          && exit.Door.State != DoorState.Locked);
        }

        return null;
    }

    private static NpcState ParseNpcState(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return NpcState.Friendly;

        string normalized = state.Trim().ToLowerInvariant();
        return normalized switch
        {
            "friendly" or "alive" => NpcState.Friendly,
            "hostile" => NpcState.Hostile,
            "neutral" or "dormant" or "passive" => NpcState.Neutral,
            "dead" or "defeated" => NpcState.Dead,
            _ => NpcState.Friendly
        };
    }

    private static bool TryParseArchetype(string archetype, out CharacterArchetype value)
    {
        if (string.IsNullOrWhiteSpace(archetype))
        {
            value = default;
            return false;
        }

        return Enum.TryParse(archetype.Trim(), true, out value);
    }

    private static bool TryParseJourneyStage(int index, out JourneyStage stage)
    {
        if (!Enum.IsDefined(typeof(JourneyStage), index))
        {
            stage = default;
            return false;
        }

        stage = (JourneyStage)index;
        return true;
    }

    private static INpcMovement ParseNpcMovement(string movement, AdventureDslContext context)
    {
        if (string.IsNullOrWhiteSpace(movement))
            return new NoNpcMovement();

        string raw = movement.Trim();
        if (raw.Equals("none", StringComparison.OrdinalIgnoreCase))
            return new NoNpcMovement();

        if (raw.Equals("random", StringComparison.OrdinalIgnoreCase))
            return new RandomNpcMovement();

        if (raw.StartsWith("patrol:", StringComparison.OrdinalIgnoreCase))
        {
            string routeText = raw["patrol:".Length..];
            List<ILocation> route = [];
            foreach (string token in routeText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                string locationId = NormalizeId(token);
                if (context.Locations.TryGetValue(locationId, out Location? location))
                    route.Add(location);
            }

            if (route.Count > 0)
                return new PatrolNpcMovement(route);
        }

        return new NoNpcMovement();
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

    private void HandleNpc(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string npcId = NormalizeId(parts[0]);
        var npc = new DslNpcDef { Id = npcId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("name="))
                npc.Name = parts[i][5..];
            else if (parts[i].StartsWith("state="))
                npc.State = parts[i][6..];
            else if (parts[i].StartsWith("health="))
            {
                if (int.TryParse(parts[i][7..], out var health))
                    npc.Health = health;
            }
            else if (parts[i].StartsWith("archetype="))
                npc.Archetype = parts[i][10..];
            else if (parts[i].StartsWith("dies_at="))
            {
                if (int.TryParse(parts[i][8..], out var dies))
                    npc.DiesAt = dies;
            }
            else if (parts[i].StartsWith("movement="))
                npc.Movement = parts[i][9..];
            else if (parts[i].StartsWith("description="))
                npc.Description = parts[i][12..];
        }

        _npcDefs.Add(npc);
    }

    private void HandleNpcPlace(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string locationId = NormalizeId(parts[0]);
        string npcId = NormalizeId(parts[1]);

        _npcPlacements.Add(new DslNpcPlacement { LocationId = locationId, NpcId = npcId });
    }

    private void HandleNpcDialog(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string npcId = NormalizeId(parts[0]);
        var dialog = new DslNpcDialog { NpcId = npcId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("text="))
                dialog.Text = parts[i][5..];
        }

        _npcDialogs.Add(dialog);
    }

    private void HandleNpcAcceptance(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string npcId = NormalizeId(parts[0]);
        var rule = new DslNpcAcceptanceRule { NpcId = npcId, Priority = _acceptanceRulePriority++ };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("target="))
                rule.TargetId = parts[i][7..];
            else if (parts[i].StartsWith("if="))
                rule.Condition = parts[i][3..];
            else if (parts[i].StartsWith("level="))
                rule.Level = parts[i][6..];
            else if (parts[i].StartsWith("say="))
                rule.Say = parts[i][4..];
        }

        if (!string.IsNullOrEmpty(rule.TargetId) && !string.IsNullOrEmpty(rule.Level))
            _acceptanceRules.Add(rule);
    }

    private void HandleNpcAcceptanceDefault(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string npcId = NormalizeId(parts[0]);
        var def = new DslNpcAcceptanceDefault { NpcId = npcId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("target="))
                def.TargetId = parts[i][7..];
            else if (parts[i].StartsWith("level="))
                def.Level = parts[i][6..];
            else if (parts[i].StartsWith("say="))
                def.Say = parts[i][4..];
        }

        _acceptanceDefaults.Add(def);
    }

    private void HandleNpcDialogOption(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string npcId = NormalizeId(parts[0]);
        var option = new DslNpcDialogOption { NpcId = npcId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("from="))
                option.FromNodeId = parts[i][5..];
            else if (parts[i].StartsWith("text="))
                option.OptionText = parts[i][5..];
            else if (parts[i].StartsWith("to="))
                option.ToNodeId = parts[i][3..];
        }

        if (!string.IsNullOrEmpty(option.FromNodeId) && !string.IsNullOrEmpty(option.ToNodeId))
            _dialogOptions.Add(option);
    }

    private void HandleNpcRule(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string npcId = NormalizeId(parts[0]);
        var rule = new DslNpcRule { NpcId = npcId, Priority = _npcRulePriority-- };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("id="))
                rule.RuleId = parts[i][3..];
            else if (parts[i].StartsWith("if="))
                rule.Condition = parts[i][3..];
            else if (parts[i].StartsWith("priority="))
            {
                if (int.TryParse(parts[i][9..], out var p))
                    rule.Priority = p;
            }
            else if (parts[i].StartsWith("say="))
                rule.Say = parts[i][4..];
            else if (parts[i].StartsWith("then="))
                rule.Then = parts[i][5..];
        }

        _npcRules.Add(rule);
    }

    private void HandleNpcTrigger(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string npcId = NormalizeId(parts[0]);
        var trigger = new DslNpcTrigger { NpcId = npcId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("sense="))
                trigger.Sense = parts[i][6..];
            else if (parts[i].StartsWith("target="))
                trigger.Target = parts[i][7..];
            else if (parts[i].StartsWith("after="))
            {
                if (int.TryParse(parts[i][6..], out var ticks))
                    trigger.After = ticks;
            }
            else if (parts[i].StartsWith("say="))
                trigger.Say = parts[i][4..];
            else if (parts[i].Equals("say_once=true", StringComparison.OrdinalIgnoreCase))
                trigger.SayOnce = true;
            else if (parts[i].Equals("flee=true", StringComparison.OrdinalIgnoreCase))
                trigger.Flee = true;
        }

        if (!string.IsNullOrEmpty(trigger.Sense) && !string.IsNullOrEmpty(trigger.Target))
            _npcTriggers.Add(trigger);
    }

    private void HandleQuest(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string questId = NormalizeId(parts[0]);
        var quest = new DslQuest { Id = questId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("title="))
                quest.Title = parts[i][6..];
            else if (parts[i].StartsWith("desc="))
                quest.Description = parts[i][5..];
            else if (parts[i].StartsWith("state="))
                quest.State = parts[i][6..];
        }

        _quests.Add(quest);
    }

    private void HandleQuestStage(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string questId = NormalizeId(parts[0]);
        var stage = new DslQuestStage { QuestId = questId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("id="))
                stage.StageId = parts[i][3..];
            else if (parts[i].StartsWith("required="))
                stage.Required = parts[i][9..].Split(',').Select(x => NormalizeId(x.Trim())).ToList();
            else if (parts[i].StartsWith("optional="))
                stage.Optional = parts[i][9..].Split(',').Select(x => NormalizeId(x.Trim())).ToList();
        }

        if (!string.IsNullOrEmpty(stage.StageId))
            _questStages.Add(stage);
    }

    private void HandleQuestObjective(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string questId = NormalizeId(parts[0]);
        var objective = new DslQuestObjective { QuestId = questId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("id="))
                objective.ObjectiveId = parts[i][3..];
            else if (parts[i].StartsWith("title="))
                objective.Title = parts[i][6..];
            else if (parts[i].StartsWith("desc="))
                objective.Description = parts[i][5..];
        }

        if (!string.IsNullOrEmpty(objective.ObjectiveId))
            _questObjectives.Add(objective);
    }

    private void HandleQuestCondition(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string questId = NormalizeId(parts[0]);
        string expr = string.Join(" ", parts[1..]);

        _questConditions.Add(new DslQuestCondition { QuestId = questId, Expression = expr });
    }

    private void HandleQuestOnComplete(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string questId = NormalizeId(parts[0]);
        string effects = string.Join(" ", parts[1..]);

        _questOnCompletes.Add(new DslQuestOnComplete { QuestId = questId, Effects = effects });
    }

    private void HandleQuestOnFail(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string questId = NormalizeId(parts[0]);
        string effects = string.Join(" ", parts[1..]);

        _questOnFails.Add(new DslQuestOnFail { QuestId = questId, Effects = effects });
    }

    private void HandleTrigger(AdventureDslContext context, string value, string triggerType)
    {
        var parts = SplitParts(value);
        var trigger = new DslTrigger { TriggerType = triggerType };

        // Context can be location_id, item_id, etc. depending on trigger type
        if (parts.Count > 0 && !parts[0].Contains("="))
            trigger.Context = NormalizeId(parts[0]);

        for (int i = (trigger.Context != "" ? 1 : 0); i < parts.Count; i++)
        {
            if (parts[i].StartsWith("if="))
                trigger.Condition = parts[i][3..];
            else if (parts[i].StartsWith("effects="))
                trigger.Effects = parts[i][8..];
        }

        if (!string.IsNullOrEmpty(trigger.Effects))
            _triggers.Add(trigger);
    }

    private void HandleScheduleAt(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        if (int.TryParse(parts[0], out var tick))
        {
            var schedule = new DslSchedule { Id = $"schedule_{_scheduleId++}", ScheduleType = "at", TickValue = tick };

            for (int i = 1; i < parts.Count; i++)
            {
                if (parts[i].StartsWith("effects="))
                    schedule.Effects = parts[i][8..];
            }

            if (!string.IsNullOrEmpty(schedule.Effects))
                _schedules.Add(schedule);
        }
    }

    private void HandleScheduleEvery(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        if (int.TryParse(parts[0], out var ticks))
        {
            var schedule = new DslSchedule { Id = $"schedule_{_scheduleId++}", ScheduleType = "every", TickValue = ticks };

            for (int i = 1; i < parts.Count; i++)
            {
                if (parts[i].StartsWith("effects="))
                    schedule.Effects = parts[i][8..];
            }

            if (!string.IsNullOrEmpty(schedule.Effects))
                _schedules.Add(schedule);
        }
    }

    private void HandleScheduleWhen(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        var schedule = new DslSchedule { Id = $"schedule_{_scheduleId++}", ScheduleType = "when" };

        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("if="))
                schedule.Condition = parts[i][3..];
            else if (parts[i].StartsWith("effects="))
                schedule.Effects = parts[i][8..];
        }

        if (!string.IsNullOrEmpty(schedule.Condition) && !string.IsNullOrEmpty(schedule.Effects))
            _schedules.Add(schedule);
    }

    private void HandleRandomSettings(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);

        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("enabled="))
                _randomSettings.Enabled = parts[i][8..].Equals("true", StringComparison.OrdinalIgnoreCase);
            else if (parts[i].StartsWith("chance="))
            {
                if (float.TryParse(parts[i][7..], out var chance))
                    _randomSettings.Chance = Math.Clamp(chance, 0f, 1f);
            }
        }
    }

    private void HandleRandomEvent(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string eventId = NormalizeId(parts[0]);
        var evt = new DslRandomEvent { Id = eventId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("weight="))
            {
                if (int.TryParse(parts[i][7..], out var w))
                    evt.Weight = w;
            }
            else if (parts[i].StartsWith("cooldown="))
            {
                if (int.TryParse(parts[i][9..], out var cd))
                    evt.Cooldown = cd;
            }
            else if (parts[i].StartsWith("if="))
                evt.Condition = parts[i][3..];
            else if (parts[i].StartsWith("effects="))
                evt.Effects = parts[i][8..];
        }

        if (!string.IsNullOrEmpty(evt.Effects))
            _randomEvents.Add(evt);
    }

    private void HandleBranch(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        _currentBranchId = NormalizeId(parts[0]);
    }

    private void HandleBranchWhen(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string branchId = NormalizeId(parts[0]);
        var branch = new DslBranch { Id = branchId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].Contains(">=") || parts[i].Contains("<=") || parts[i].Contains(":"))
                branch.Condition = parts[i];
        }

        _storySystem.AddBranch(branch);
    }

    private void HandleBranchThen(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string branchId = NormalizeId(parts[0]);
        var branch = _storySystem.GetBranch(branchId);

        if (branch != null)
        {
            for (int i = 1; i < parts.Count; i++)
            {
                if (parts[i].StartsWith("effects="))
                    branch.Effects = parts[i][8..];
            }
        }
    }

    private void HandleChapter(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        string chapterId = NormalizeId(parts[0]);
        _currentChapterId = chapterId;
        var chapter = new DslChapter { Id = chapterId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("title="))
                chapter.Title = parts[i][6..];
            else if (parts[i].StartsWith("desc="))
                chapter.Description = parts[i][5..];
        }

        _storySystem.AddChapter(chapter);
    }

    private void HandleChapterObjective(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string objId = NormalizeId(parts[0]);
        var obj = new DslChapterObjective { ChapterId = _currentChapterId, ObjectiveId = objId };

        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("title="))
                obj.Title = parts[i][6..];
            else if (parts[i].StartsWith("desc="))
                obj.Description = parts[i][5..];
        }

        _storySystem.AddObjective(obj);

        // Add to current chapter
        var chapter = _storySystem.GetChapter(_currentChapterId);
        if (chapter != null && !chapter.Objectives.Contains(objId))
            chapter.Objectives.Add(objId);
    }

    private void HandleChapterNext(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 2) return;

        string nextChapterId = NormalizeId(parts[1]);
        var chapter = _storySystem.GetChapter(_currentChapterId);

        if (chapter != null && !chapter.NextChapters.Contains(nextChapterId))
            chapter.NextChapters.Add(nextChapterId);
    }

    private void HandleChapterEnd(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        var chapter = _storySystem.GetChapter(_currentChapterId);
        if (chapter != null)
        {
            chapter.IsEnding = true;

            var ending = new DslChapterEnding { ChapterId = _currentChapterId };

            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i].StartsWith("title="))
                    ending.Title = parts[i][6..];
                else if (parts[i].StartsWith("desc="))
                    ending.Description = parts[i][5..];
            }

            _storySystem.AddEnding(ending);
        }
    }

    private void HandleChapterUnlockIf(AdventureDslContext context, string value)
    {
        var chapter = _storySystem.GetChapter(_currentChapterId);
        if (chapter != null)
            chapter.UnlockCondition = value.Trim();
    }

    private void HandleParserOption(AdventureDslContext context, string value)
    {
        var parts = value.Split('=');
        if (parts.Length != 2) return;

        var option = new DslParserOption
        {
            Key = parts[0].Trim(),
            Value = parts[1].Trim()
        };
        _parserConfig.Options.Add(option);
    }

    private void HandleCommandAlias(AdventureDslContext context, string value)
    {
        var parts = value.Split('=');
        if (parts.Length != 2) return;

        var alias = new DslCommandAlias
        {
            Alias = parts[0].Trim(),
            TargetCommand = parts[1].Trim()
        };
        _parserConfig.CommandAliases.Add(alias);
    }

    private void HandleDirectionAlias(AdventureDslContext context, string value)
    {
        var parts = value.Split('=');
        if (parts.Length != 2) return;

        var alias = new DslDirectionAlias
        {
            Alias = parts[0].Trim(),
            TargetDirection = parts[1].Trim()
        };
        _parserConfig.DirectionAliases.Add(alias);
    }

    private void HandleCustomCommand(AdventureDslContext context, string value)
    {
        foreach (string verb in value.Split(','))
        {
            string v = verb.Trim().ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(v))
                _parserConfig.CustomVerbs.Add(new DslCustomVerb { Verb = v });
        }
    }

    private void HandleNpcReaction(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 1) return;

        var reaction = new DslNpcReaction { NpcId = NormalizeId(parts[0]) };
        for (int i = 1; i < parts.Count; i++)
        {
            if (parts[i].StartsWith("on=", StringComparison.OrdinalIgnoreCase))
            {
                // Support "on=talk,has_item=brass_key,door_unlocked=hatch" — first token is trigger, rest are inline conditions
                string[] onTokens = parts[i][3..].Trim().ToLowerInvariant().Split(',');
                reaction.Trigger = onTokens[0].Trim();
                for (int t = 1; t < onTokens.Length; t++)
                {
                    string inlineCond = onTokens[t].Trim();
                    if (!string.IsNullOrWhiteSpace(inlineCond))
                        reaction.Condition = string.IsNullOrWhiteSpace(reaction.Condition)
                            ? inlineCond
                            : reaction.Condition + "," + inlineCond;
                }
            }
            else if (parts[i].StartsWith("text=", StringComparison.OrdinalIgnoreCase))
                reaction.Text = parts[i][5..].Trim();
            else if (parts[i].StartsWith("if=", StringComparison.OrdinalIgnoreCase))
                reaction.Condition = string.IsNullOrWhiteSpace(reaction.Condition)
                    ? parts[i][3..].Trim()
                    : reaction.Condition + "," + parts[i][3..].Trim();
            else if (parts[i].Equals("end_game=true", StringComparison.OrdinalIgnoreCase))
                reaction.EndGame = true;
            else if (parts[i].StartsWith("set_flag=", StringComparison.OrdinalIgnoreCase))
            {
                string[] kv = parts[i][9..].Split(':');
                if (kv.Length == 2)
                    reaction.SetFlags[kv[0].Trim()] = kv[1].Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            else if (parts[i].StartsWith("inc_counter=", StringComparison.OrdinalIgnoreCase))
            {
                string[] kv = parts[i][12..].Split(':');
                if (kv.Length == 2 && int.TryParse(kv[1].Trim(), out int val))
                    reaction.IncrementCounters[kv[0].Trim()] = val;
            }
        }

        if (!string.IsNullOrWhiteSpace(reaction.NpcId) && !string.IsNullOrWhiteSpace(reaction.Trigger) && !string.IsNullOrWhiteSpace(reaction.Text))
            _parserConfig.NpcReactions.Add(reaction);
    }

    private static Action<IGameState>? BuildNpcReactionEffect(DslNpcReaction reaction)
    {
        if (reaction.SetFlags.Count == 0 && reaction.IncrementCounters.Count == 0)
            return null;

        var flags = new Dictionary<string, bool>(reaction.SetFlags);
        var counters = new Dictionary<string, int>(reaction.IncrementCounters);

        return state =>
        {
            foreach (var (key, val) in flags)
                state.WorldState.SetFlag(key, val);
            foreach (var (key, amount) in counters)
                state.WorldState.Increment(key, amount);
        };
    }

    private void HandleNpcIdle(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count < 3) return; // npc_id | interval | message1 [| message2 ...]

        if (!int.TryParse(parts[1].Trim(), out int interval) || interval < 1) return;

        var idle = new DslNpcIdleBehavior
        {
            NpcId = NormalizeId(parts[0]),
            Interval = interval,
            Messages = parts.Skip(2).Select(m => m.Trim()).Where(m => m.Length > 0).ToList()
        };

        if (idle.Messages.Count > 0)
            _npcIdleBehaviors.Add(idle);
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
    public IReadOnlyList<DslNpcDef> GetNpcDefs() => _npcDefs;
    public IReadOnlyList<DslNpcPlacement> GetNpcPlacements() => _npcPlacements;
    public IReadOnlyList<DslNpcDialog> GetNpcDialogs() => _npcDialogs;
    public IReadOnlyList<DslNpcIdleBehavior> GetNpcIdleBehaviors() => _npcIdleBehaviors;
    public IReadOnlyList<DslNpcAcceptanceRule> GetAcceptanceRules() => _acceptanceRules;
    public IReadOnlyList<DslNpcAcceptanceDefault> GetAcceptanceDefaults() => _acceptanceDefaults;
    public IReadOnlyList<DslNpcDialogOption> GetDialogOptions() => _dialogOptions;
    public IReadOnlyList<DslNpcRule> GetNpcRules() => _npcRules;
    public IReadOnlyList<DslNpcTrigger> GetNpcTriggers() => _npcTriggers;
    public IReadOnlyList<DslQuest> GetQuests() => _quests;
    public IReadOnlyList<DslQuestStage> GetQuestStages() => _questStages;
    public IReadOnlyList<DslQuestObjective> GetQuestObjectives() => _questObjectives;
    public IReadOnlyList<DslQuestCondition> GetQuestConditions() => _questConditions;
    public IReadOnlyList<DslQuestOnComplete> GetQuestOnCompletes() => _questOnCompletes;
    public IReadOnlyList<DslQuestOnFail> GetQuestOnFails() => _questOnFails;
    public IReadOnlyList<DslTrigger> GetTriggers() => _triggers;
    public IReadOnlyList<DslSchedule> GetSchedules() => _schedules;
    public DslRandomSettings GetRandomSettings() => _randomSettings;
    public IReadOnlyList<DslRandomEvent> GetRandomEvents() => _randomEvents;
    public DslStorySystem GetStorySystem() => _storySystem;
    public DslParserConfiguration GetParserConfiguration() => _parserConfig;
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

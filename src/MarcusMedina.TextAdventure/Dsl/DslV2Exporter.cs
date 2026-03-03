// <copyright file="DslV2Exporter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text;

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// Exports DSL v2 parser data structures back to DSL format.
/// Supports round-trip compatibility (parse -> export -> parse = equivalent).
/// </summary>
public sealed class DslV2Exporter
{
    /// <summary>
    /// Export a DslParser instance back to DSL v2 format.
    /// </summary>
    public string Export(DslParser parser)
    {
        ArgumentNullException.ThrowIfNull(parser);

        var sb = new StringBuilder();

        // Entity definitions
        ExportDefinedItems(sb, parser);
        ExportDefinedNpcs(sb, parser);

        // Start state
        ExportStartState(sb, parser);

        // Item reactions and recipes
        ExportItemReactions(sb, parser);
        ExportItemConsequences(sb, parser);
        ExportRecipes(sb, parser);

        // World interaction
        ExportDoorConfigs(sb, parser);
        ExportExitConfigs(sb, parser);
        ExportRoomDescriptions(sb, parser);
        ExportRoomVariables(sb, parser);
        ExportRoomTransforms(sb, parser);

        // NPC system
        ExportNpcDefinitions(sb, parser);
        ExportNpcRulesAndTriggers(sb, parser);

        // Quests
        ExportQuests(sb, parser);

        // Events and schedules
        ExportTriggers(sb, parser);
        ExportSchedules(sb, parser);
        ExportRandomEvents(sb, parser);

        // Story system
        ExportStoryBranches(sb, parser);
        ExportChapters(sb, parser);

        // Parser configuration
        ExportParserConfiguration(sb, parser);

        return sb.ToString();
    }

    /// <summary>
    /// Export to file.
    /// </summary>
    public void ExportToFile(DslParser parser, string path)
    {
        ArgumentNullException.ThrowIfNull(parser);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var content = Export(parser);
        File.WriteAllText(path, content);
    }

    private static void ExportDefinedItems(StringBuilder sb, DslParser parser)
    {
        var items = parser.GetDefinedItems();
        if (items.Count == 0) return;

        sb.AppendLine("// Defined items");
        foreach (var (id, def) in items.OrderBy(x => x.Key))
        {
            if (def.IsKey || def.IsDoor) continue;
            ExportEntityDefinition(sb, "define item", id, def);
        }
        sb.AppendLine();
    }

    private static void ExportDefinedNpcs(StringBuilder sb, DslParser parser)
    {
        var npcs = parser.GetDefinedNpcs();
        if (npcs.Count == 0) return;

        sb.AppendLine("// Defined NPCs");
        foreach (var (id, def) in npcs.OrderBy(x => x.Key))
        {
            ExportEntityDefinition(sb, "define npc", id, def);
        }
        sb.AppendLine();
    }

    private static void ExportEntityDefinition(StringBuilder sb, string keyword, string id, DslEntityDefinition def)
    {
        var parts = new List<string> { id, def.Name };

        if (!string.IsNullOrEmpty(def.Description))
            parts.Add(def.Description);

        var line = string.Join(" | ", parts);

        if (def.Options.Count > 0)
        {
            var options = string.Join(" | ", def.Options.OrderBy(x => x.Key).Select(x => $"{x.Key}={x.Value}"));
            line += $" | {options}";
        }

        sb.AppendLine($"{keyword}: {line}");
    }

    private static void ExportStartState(StringBuilder sb, DslParser parser)
    {
        var state = parser.GetStartState();
        if (string.IsNullOrEmpty(state.CurrentLocationId) && state.StartInventory.Count == 0) return;

        sb.AppendLine("// Start state");
        if (!string.IsNullOrEmpty(state.CurrentLocationId))
            sb.AppendLine($"current_location: {state.CurrentLocationId}");

        foreach (var inv in state.StartInventory.OrderBy(x => x.ItemId))
            sb.AppendLine($"start_inventory: {inv.ItemId}{(inv.Amount > 1 ? $" | amount={inv.Amount}" : "")}");

        foreach (var (key, value) in state.Flags.OrderBy(x => x.Key))
            sb.AppendLine($"flag: {key}={value}");

        foreach (var (key, value) in state.Counters.OrderBy(x => x.Key))
            sb.AppendLine($"counter: {key}={value}");

        foreach (var (key, value) in state.Relationships.OrderBy(x => x.Key))
            sb.AppendLine($"relationship: {key}={value}");

        sb.AppendLine();
    }

    private static void ExportItemReactions(StringBuilder sb, DslParser parser)
    {
        var reactions = parser.GetItemReactions();
        if (reactions.Count == 0) return;

        sb.AppendLine("// Item reactions");
        foreach (var reaction in reactions.OrderBy(x => x.ItemId))
            sb.AppendLine($"item_reaction: {reaction.ItemId} | {reaction.Action} | {reaction.Text}");
        sb.AppendLine();
    }

    private static void ExportItemConsequences(StringBuilder sb, DslParser parser)
    {
        var consequences = parser.GetItemConsequences();
        if (consequences.Count == 0) return;

        sb.AppendLine("// Item consequences");
        foreach (var consequence in consequences.OrderBy(x => x.ItemId))
        {
            var parts = new List<string> { consequence.ItemId, consequence.Action };
            if (consequence.Destroy) parts.Add("destroy");
            if (consequence.Create.Count > 0) parts.Add($"create={string.Join(",", consequence.Create)}");
            if (!string.IsNullOrEmpty(consequence.Transform)) parts.Add($"transform={consequence.Transform}");
            if (!string.IsNullOrEmpty(consequence.Message)) parts.Add($"message={consequence.Message}");

            sb.AppendLine($"item_consequence: {string.Join(" | ", parts)}");
        }
        sb.AppendLine();
    }

    private static void ExportRecipes(StringBuilder sb, DslParser parser)
    {
        var recipes = parser.GetRecipes();
        if (recipes.Count == 0) return;

        sb.AppendLine("// Recipes");
        foreach (var recipe in recipes.OrderBy(x => x.Id))
            sb.AppendLine($"recipe: {recipe.Id} | {recipe.LeftItemId} + {recipe.RightItemId} = {recipe.CreatedItemId}");
        sb.AppendLine();
    }

    private static void ExportDoorConfigs(StringBuilder sb, DslParser parser)
    {
        var doors = parser.GetDoorConfigs();
        if (doors.Count == 0) return;

        sb.AppendLine("// Door configurations");
        foreach (var door in doors.OrderBy(x => x.Id))
        {
            var parts = new List<string> { door.Id, door.State };
            if (door.Aliases.Count > 0) parts.Add($"aliases={string.Join(",", door.Aliases)}");

            sb.AppendLine($"door_config: {string.Join(" | ", parts)}");
        }
        sb.AppendLine();
    }

    private static void ExportExitConfigs(StringBuilder sb, DslParser parser)
    {
        var exits = parser.GetExitConfigs();
        if (exits.Count == 0) return;

        sb.AppendLine("// Exit configurations");
        foreach (var exit in exits.OrderBy(x => $"{x.FromLocationId}:{x.Direction}"))
        {
            var parts = new List<string> { exit.Direction, $"{exit.FromLocationId}->{exit.ToLocationId}" };
            if (exit.Hidden) parts.Add("hidden");
            if (!string.IsNullOrEmpty(exit.DiscoverIf)) parts.Add($"discover_if={exit.DiscoverIf}");
            if (exit.Perception != 50) parts.Add($"perception={exit.Perception}");
            if (!string.IsNullOrEmpty(exit.DoorId)) parts.Add($"door={exit.DoorId}");
            if (exit.OneWay) parts.Add("oneway");

            sb.AppendLine($"exit_config: {string.Join(" | ", parts)}");
        }
        sb.AppendLine();
    }

    private static void ExportRoomDescriptions(StringBuilder sb, DslParser parser)
    {
        var descs = parser.GetRoomDescriptions();
        if (descs.Count == 0) return;

        sb.AppendLine("// Room descriptions");
        foreach (var desc in descs.OrderBy(x => x.LocationId))
        {
            if (!string.IsNullOrEmpty(desc.DefaultDescription))
                sb.AppendLine($"room_desc: {desc.LocationId} | {desc.DefaultDescription}");
        }
        sb.AppendLine();
    }

    private static void ExportRoomVariables(StringBuilder sb, DslParser parser)
    {
        var vars = parser.GetRoomVariables();
        if (vars.Count == 0) return;

        sb.AppendLine("// Room variables");
        foreach (var variable in vars.OrderBy(x => x.LocationId))
            sb.AppendLine($"room_var: {variable.LocationId} | {variable.Key}={variable.Value}");
        sb.AppendLine();
    }

    private static void ExportRoomTransforms(StringBuilder sb, DslParser parser)
    {
        var transforms = parser.GetRoomTransforms();
        if (transforms.Count == 0) return;

        sb.AppendLine("// Room transforms");
        foreach (var transform in transforms.OrderBy(x => x.SourceLocationId))
        {
            var parts = new List<string> { $"{transform.SourceLocationId}->{transform.TargetLocationId}" };
            if (!string.IsNullOrEmpty(transform.Condition)) parts.Add($"when={transform.Condition}");
            if (transform.Irreversible) parts.Add("irreversible");

            sb.AppendLine($"room_transform: {string.Join(" | ", parts)}");
        }
        sb.AppendLine();
    }

    private static void ExportNpcDefinitions(StringBuilder sb, DslParser parser)
    {
        var npcs = parser.GetNpcDefs();
        if (npcs.Count == 0) return;

        sb.AppendLine("// NPC definitions");
        foreach (var npc in npcs.OrderBy(x => x.Id))
        {
            var parts = new List<string> { npc.Id };
            if (!string.IsNullOrEmpty(npc.State)) parts.Add($"state={npc.State}");
            if (npc.Health > 0) parts.Add($"health={npc.Health}");
            if (!string.IsNullOrEmpty(npc.Archetype)) parts.Add($"archetype={npc.Archetype}");

            sb.AppendLine($"npc: {string.Join(" | ", parts)}");
        }
        sb.AppendLine();
    }

    private static void ExportNpcRulesAndTriggers(StringBuilder sb, DslParser parser)
    {
        var rules = parser.GetNpcRules();
        var triggers = parser.GetNpcTriggers();

        if (rules.Count == 0 && triggers.Count == 0) return;

        sb.AppendLine("// NPC rules and triggers");

        foreach (var rule in rules.OrderBy(x => x.NpcId).ThenBy(x => x.Priority))
        {
            var parts = new List<string> { rule.NpcId };
            if (!string.IsNullOrEmpty(rule.Condition)) parts.Add($"when={rule.Condition}");
            parts.Add($"priority={rule.Priority}");
            if (!string.IsNullOrEmpty(rule.Say)) parts.Add($"say={rule.Say}");
            if (!string.IsNullOrEmpty(rule.Then)) parts.Add($"then={rule.Then}");

            sb.AppendLine($"npc_rule: {string.Join(" | ", parts)}");
        }

        foreach (var trigger in triggers.OrderBy(x => x.NpcId))
        {
            var parts = new List<string> { trigger.NpcId };
            if (!string.IsNullOrEmpty(trigger.Sense)) parts.Add($"sense={trigger.Sense}");
            if (!string.IsNullOrEmpty(trigger.Target)) parts.Add($"target={trigger.Target}");
            if (trigger.After > 0) parts.Add($"after={trigger.After}");
            if (!string.IsNullOrEmpty(trigger.Say)) parts.Add($"say={trigger.Say}");

            sb.AppendLine($"npc_trigger: {string.Join(" | ", parts)}");
        }

        sb.AppendLine();
    }

    private static void ExportQuests(StringBuilder sb, DslParser parser)
    {
        var quests = parser.GetQuests();
        if (quests.Count == 0) return;

        sb.AppendLine("// Quests");
        foreach (var quest in quests.OrderBy(x => x.Id))
            sb.AppendLine($"quest: {quest.Id} | {quest.Title} | {quest.Description}");

        var stages = parser.GetQuestStages();
        foreach (var stage in stages.OrderBy(x => x.QuestId))
        {
            var parts = new List<string> { stage.QuestId, stage.StageId };
            if (stage.Required.Count > 0) parts.Add($"required={string.Join(",", stage.Required)}");
            if (stage.Optional.Count > 0) parts.Add($"optional={string.Join(",", stage.Optional)}");

            sb.AppendLine($"quest_stage: {string.Join(" | ", parts)}");
        }

        sb.AppendLine();
    }

    private static void ExportTriggers(StringBuilder sb, DslParser parser)
    {
        var triggers = parser.GetTriggers();
        if (triggers.Count == 0) return;

        sb.AppendLine("// Triggers");
        foreach (var trigger in triggers.OrderBy(x => x.TriggerType))
        {
            var parts = new List<string> { trigger.TriggerType };
            if (!string.IsNullOrEmpty(trigger.Condition)) parts.Add($"when={trigger.Condition}");
            if (!string.IsNullOrEmpty(trigger.Effects)) parts.Add($"then={trigger.Effects}");
            if (!string.IsNullOrEmpty(trigger.Context)) parts.Add($"context={trigger.Context}");

            sb.AppendLine($"{trigger.TriggerType}: {string.Join(" | ", parts)}");
        }
        sb.AppendLine();
    }

    private static void ExportSchedules(StringBuilder sb, DslParser parser)
    {
        var schedules = parser.GetSchedules();
        if (schedules.Count == 0) return;

        sb.AppendLine("// Schedules");
        foreach (var schedule in schedules.OrderBy(x => x.Id))
        {
            var parts = new List<string> { schedule.ScheduleType };
            if (schedule.TickValue > 0) parts.Add($"tick={schedule.TickValue}");
            if (!string.IsNullOrEmpty(schedule.Condition)) parts.Add($"when={schedule.Condition}");
            if (!string.IsNullOrEmpty(schedule.Effects)) parts.Add($"then={schedule.Effects}");

            sb.AppendLine($"schedule_{schedule.ScheduleType}: {string.Join(" | ", parts)}");
        }
        sb.AppendLine();
    }

    private static void ExportRandomEvents(StringBuilder sb, DslParser parser)
    {
        var settings = parser.GetRandomSettings();
        var events = parser.GetRandomEvents();

        if (!settings.Enabled && events.Count == 0) return;

        sb.AppendLine("// Random events");
        if (!settings.Enabled)
            sb.AppendLine($"random_settings: enabled=false | chance={settings.Chance}");

        foreach (var evt in events.OrderBy(x => x.Id))
        {
            var parts = new List<string> { evt.Id, $"weight={evt.Weight}" };
            if (evt.Cooldown > 0) parts.Add($"cooldown={evt.Cooldown}");
            if (!string.IsNullOrEmpty(evt.Condition)) parts.Add($"when={evt.Condition}");
            if (!string.IsNullOrEmpty(evt.Effects)) parts.Add($"then={evt.Effects}");

            sb.AppendLine($"random_event: {string.Join(" | ", parts)}");
        }
        sb.AppendLine();
    }

    private static void ExportStoryBranches(StringBuilder sb, DslParser parser)
    {
        var system = parser.GetStorySystem();
        var branches = system.GetAllBranches().ToList();

        if (branches.Count == 0) return;

        sb.AppendLine("// Story branches");
        foreach (var branch in branches.OrderBy(x => x.Id))
        {
            var parts = new List<string> { branch.Id };
            if (!string.IsNullOrEmpty(branch.Condition)) parts.Add($"when={branch.Condition}");
            if (!string.IsNullOrEmpty(branch.Effects)) parts.Add($"then={branch.Effects}");

            sb.AppendLine($"branch: {string.Join(" | ", parts)}");
        }
        sb.AppendLine();
    }

    private static void ExportChapters(StringBuilder sb, DslParser parser)
    {
        var system = parser.GetStorySystem();
        var chapters = system.GetAllChapters().ToList();

        if (chapters.Count == 0) return;

        sb.AppendLine("// Chapters");
        foreach (var chapter in chapters.OrderBy(x => x.Id))
        {
            var parts = new List<string> { chapter.Id, chapter.Title };
            if (!string.IsNullOrEmpty(chapter.Description)) parts.Add($"desc={chapter.Description}");
            if (chapter.Objectives.Count > 0) parts.Add($"objectives={string.Join(",", chapter.Objectives)}");
            if (chapter.NextChapters.Count > 0) parts.Add($"next={string.Join(",", chapter.NextChapters)}");
            if (chapter.IsEnding) parts.Add("is_ending=true");
            if (!string.IsNullOrEmpty(chapter.UnlockCondition)) parts.Add($"unlock_if={chapter.UnlockCondition}");

            sb.AppendLine($"chapter: {string.Join(" | ", parts)}");
        }

        var endings = system.GetAllEndings().ToList();
        if (endings.Count > 0)
        {
            foreach (var ending in endings.OrderBy(x => x.ChapterId))
                sb.AppendLine($"chapter_end: {ending.ChapterId} | {ending.Title} | {ending.Description}");
        }

        sb.AppendLine();
    }

    private static void ExportParserConfiguration(StringBuilder sb, DslParser parser)
    {
        var config = parser.GetParserConfiguration();
        if (config.Options.Count == 0 && config.CommandAliases.Count == 0 && config.DirectionAliases.Count == 0)
            return;

        sb.AppendLine("// Parser configuration");

        foreach (var option in config.Options.OrderBy(x => x.Key))
            sb.AppendLine($"parser_option: {option.Key}={option.Value}");

        foreach (var alias in config.CommandAliases.OrderBy(x => x.Alias))
            sb.AppendLine($"command_alias: {alias.Alias}={alias.TargetCommand}");

        foreach (var alias in config.DirectionAliases.OrderBy(x => x.Alias))
            sb.AppendLine($"direction_alias: {alias.Alias}={alias.TargetDirection}");

        sb.AppendLine();
    }
}

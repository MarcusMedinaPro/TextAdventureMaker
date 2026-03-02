// <copyright file="DslEventAndSchedule.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 event and schedule models (Slice 080).
/// </summary>

/// <summary>
/// Generic trigger definition for DSL v2.
/// </summary>
public sealed class DslTrigger
{
    public string TriggerType { get; set; } = ""; // on_enter, on_pickup, on_talk, on_action, on_npc_death, on_tick
    public string Condition { get; set; } = ""; // Optional condition
    public string Effects { get; set; } = ""; // Effects to execute
    public string Context { get; set; } = ""; // e.g., location_id, item_id, npc_id depending on trigger type
}

/// <summary>
/// Schedule definition for DSL v2.
/// </summary>
public sealed class DslSchedule
{
    public string Id { get; set; } = "";
    public string ScheduleType { get; set; } = ""; // at, every, when
    public int TickValue { get; set; } // For "at" and "every"
    public string Condition { get; set; } = ""; // For "when"
    public string Effects { get; set; } = "";
}

/// <summary>
/// Random event settings for DSL v2.
/// </summary>
public sealed class DslRandomSettings
{
    public bool Enabled { get; set; } = true;
    public float Chance { get; set; } = 0.5f; // 0..1
}

/// <summary>
/// Random event definition for DSL v2.
/// </summary>
public sealed class DslRandomEvent
{
    public string Id { get; set; } = "";
    public int Weight { get; set; } = 1;
    public int Cooldown { get; set; } = 0; // Ticks before event can fire again
    public string Condition { get; set; } = ""; // Optional condition
    public string Effects { get; set; } = "";
}

/// <summary>
/// Effect definition for DSL v2.
/// </summary>
public sealed class DslEffect
{
    public string Type { get; set; } = ""; // spawn_item, spawn_npc, open_door, move_npc, message, etc.
    public string Param1 { get; set; } = ""; // Varies by type
    public string Param2 { get; set; } = ""; // Varies by type
}

/// <summary>
/// Effect executor for DSL v2.
/// </summary>
public sealed class DslEffectExecutor
{
    /// <summary>
    /// Parse and execute an effect string.
    /// Supports: spawn_item:id:location, spawn_npc:id:location, open_door:id, move_npc:id:location, message:text
    /// </summary>
    public IEnumerable<DslEffect> ParseEffects(string effectString)
    {
        if (string.IsNullOrWhiteSpace(effectString))
            yield break;

        // Split by semicolon to support multiple effects
        var effects = effectString.Split(';').Select(e => e.Trim()).Where(e => !string.IsNullOrEmpty(e));

        foreach (var effect in effects)
        {
            var parsed = ParseEffect(effect);
            if (parsed != null)
                yield return parsed;
        }
    }

    private DslEffect? ParseEffect(string effectString)
    {
        if (string.IsNullOrWhiteSpace(effectString))
            return null;

        // Parse effect:param1:param2 format
        var parts = effectString.Split(':');
        if (parts.Length < 1)
            return null;

        var effect = new DslEffect { Type = parts[0].ToLowerInvariant() };

        if (parts.Length > 1)
            effect.Param1 = parts[1];
        if (parts.Length > 2)
            effect.Param2 = parts[2];

        return effect;
    }

    /// <summary>
    /// Validate that an effect is well-formed.
    /// </summary>
    public bool ValidateEffect(DslEffect effect)
    {
        return effect.Type switch
        {
            "spawn_item" => !string.IsNullOrEmpty(effect.Param1) && !string.IsNullOrEmpty(effect.Param2),
            "spawn_npc" => !string.IsNullOrEmpty(effect.Param1) && !string.IsNullOrEmpty(effect.Param2),
            "open_door" => !string.IsNullOrEmpty(effect.Param1),
            "move_npc" => !string.IsNullOrEmpty(effect.Param1) && !string.IsNullOrEmpty(effect.Param2),
            "message" => !string.IsNullOrEmpty(effect.Param1),
            _ => false
        };
    }
}

// <copyright file="DslSaveState.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 save state models with completeness for full game resumption (Slice 087).
/// </summary>

/// <summary>
/// Save metadata with versioning information.
/// </summary>
public sealed class DslSaveMetadata
{
    public string SaveVersion { get; set; } = "1.0";
    public string DslVersion { get; set; } = "v2";
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? GameTitle { get; set; }
    public string? PlayerName { get; set; }
    public int PlaytimeSeconds { get; set; }
}

/// <summary>
/// Door state snapshot for save/load.
/// </summary>
public sealed class DslDoorState
{
    public string DoorId { get; set; } = "";
    public string State { get; set; } = "closed"; // open, closed, locked, destroyed
    public bool Locked { get; set; }
    public bool Destroyed { get; set; }
}

/// <summary>
/// Exit discovery state for hidden exits.
/// </summary>
public sealed class DslExitDiscoveryState
{
    public string LocationId { get; set; } = "";
    public string Direction { get; set; } = "";
    public bool Discovered { get; set; }
    public DateTime? DiscoveredAt { get; set; }
}

/// <summary>
/// NPC state snapshot including location and health.
/// </summary>
public sealed class DslNpcState
{
    public string NpcId { get; set; } = "";
    public string LocationId { get; set; } = "";
    public string CurrentState { get; set; } = "idle"; // idle, alert, fled, defeated
    public int Health { get; set; } = 100;
    public bool IsDefeated { get; set; }
    public Dictionary<string, string> Properties { get; set; } = [];
}

/// <summary>
/// Quest progress snapshot.
/// </summary>
public sealed class DslQuestProgress
{
    public string QuestId { get; set; } = "";
    public string State { get; set; } = "active"; // active, complete, failed, abandoned
    public string CurrentStageId { get; set; } = "";
    public List<string> CompletedObjectives { get; set; } = [];
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Story progression state.
/// </summary>
public sealed class DslStoryProgress
{
    public string CurrentChapterId { get; set; } = "";
    public List<string> CompletedChapters { get; set; } = [];
    public List<string> DiscoveredBranches { get; set; } = [];
    public Dictionary<string, string> BranchStates { get; set; } = [];
}

/// <summary>
/// Time and tick state.
/// </summary>
public sealed class DslTimeState
{
    public int CurrentTick { get; set; }
    public int CurrentPhase { get; set; } // Day phase, cycle number, etc.
    public DateTime? InGameTime { get; set; }
}

/// <summary>
/// Random event runtime state.
/// </summary>
public sealed class DslRandomEventState
{
    public string EventId { get; set; } = "";
    public int LastTriggeredTick { get; set; } = -1;
    public int CooldownRemaining { get; set; }
    public int TimesTriggered { get; set; }
}

/// <summary>
/// Schedule cursor state.
/// </summary>
public sealed class DslScheduleState
{
    public string ScheduleId { get; set; } = "";
    public int LastExecutedTick { get; set; } = -1;
    public int NextExecutionTick { get; set; }
}

/// <summary>
/// Status effect state.
/// </summary>
public sealed class DslStatusEffect
{
    public string EffectId { get; set; } = "";
    public string TargetId { get; set; } = ""; // Player or NPC ID
    public string EffectType { get; set; } = ""; // poison, curse, buff, etc.
    public int Severity { get; set; } = 1;
    public int RemainingTicks { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = [];
}

/// <summary>
/// Complete save game state with all runtime data.
/// </summary>
public sealed class DslCompleteGameState
{
    /// <summary>
    /// Save metadata and versioning.
    /// </summary>
    public DslSaveMetadata Metadata { get; set; } = new();

    /// <summary>
    /// Core game state from start-state definition.
    /// </summary>
    public DslStartStateDefinition StartState { get; set; } = new();

    /// <summary>
    /// Current door states.
    /// </summary>
    public List<DslDoorState> DoorStates { get; set; } = [];

    /// <summary>
    /// Exit discovery progress for hidden exits.
    /// </summary>
    public List<DslExitDiscoveryState> ExitDiscoveries { get; set; } = [];

    /// <summary>
    /// NPC runtime states including location and health.
    /// </summary>
    public List<DslNpcState> NpcStates { get; set; } = [];

    /// <summary>
    /// Quest progression data.
    /// </summary>
    public List<DslQuestProgress> QuestProgress { get; set; } = [];

    /// <summary>
    /// Story and chapter progression.
    /// </summary>
    public DslStoryProgress StoryProgress { get; set; } = new();

    /// <summary>
    /// Time and phase state.
    /// </summary>
    public DslTimeState TimeState { get; set; } = new();

    /// <summary>
    /// Random event runtime states.
    /// </summary>
    public List<DslRandomEventState> RandomEventStates { get; set; } = [];

    /// <summary>
    /// Schedule execution states.
    /// </summary>
    public List<DslScheduleState> ScheduleStates { get; set; } = [];

    /// <summary>
    /// Active status effects.
    /// </summary>
    public List<DslStatusEffect> ActiveEffects { get; set; } = [];

    /// <summary>
    /// Validate save state integrity.
    /// </summary>
    public List<string> Validate()
    {
        var errors = new List<string>();

        // Check for impossible states
        if (string.IsNullOrEmpty(StartState.CurrentLocationId))
            errors.Add("Current location is required");

        // Validate NPC locations exist
        var validLocations = StartState.CurrentLocationId ?? "";
        foreach (var npc in NpcStates)
        {
            if (string.IsNullOrEmpty(npc.LocationId))
                errors.Add($"NPC {npc.NpcId} has no location");

            if (npc.Health <= 0 && !npc.IsDefeated)
                errors.Add($"NPC {npc.NpcId} has zero health but is not marked defeated");
        }

        // Validate quest states
        foreach (var quest in QuestProgress)
        {
            if (quest.State == "complete" && quest.CompletedAt is null)
                errors.Add($"Quest {quest.QuestId} is complete but has no completion time");
        }

        // Validate story state
        if (!string.IsNullOrEmpty(StoryProgress.CurrentChapterId) &&
            StoryProgress.CompletedChapters.Contains(StoryProgress.CurrentChapterId))
            errors.Add($"Chapter {StoryProgress.CurrentChapterId} is both current and completed");

        return errors;
    }
}

// <copyright file="DslSaveStateCollector.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// Collects complete save state snapshots from game state for persistent storage (Slice 087).
/// </summary>
public sealed class DslSaveStateCollector
{
    /// <summary>
    /// Create a complete save state snapshot from current game state.
    /// </summary>
    public DslCompleteGameState CollectSaveState(GameState gameState, DslParser? parser = null)
    {
        ArgumentNullException.ThrowIfNull(gameState);

        var saveState = new DslCompleteGameState
        {
            Metadata = new DslSaveMetadata
            {
                SaveVersion = "1.0",
                DslVersion = "v2",
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                PlaytimeSeconds = 0 // Would be calculated from game state
            },
            StartState = parser?.GetStartState() ?? new DslStartStateDefinition
            {
                CurrentLocationId = gameState.CurrentLocation.Id
            }
        };

        // Collect door states
        CollectDoorStates(gameState, saveState);

        // Collect exit discovery states
        CollectExitDiscoveryStates(gameState, saveState);

        // Collect NPC states (would need game engine NPC system)
        CollectNpcStates(gameState, saveState);

        // Collect quest progress (would need game state support)
        CollectQuestProgress(gameState, saveState);

        // Collect story progression (would need game state support)
        CollectStoryProgress(gameState, saveState);

        // Collect time state
        CollectTimeState(gameState, saveState);

        // Collect random event states
        CollectRandomEventStates(gameState, saveState);

        // Collect active status effects
        CollectStatusEffects(gameState, saveState);

        return saveState;
    }

    private static void CollectDoorStates(GameState gameState, DslCompleteGameState saveState)
    {
        // Collect door state from all locations
        foreach (var location in gameState.Locations)
        {
            foreach (var exit in location.Exits.Values)
            {
                if (exit.Door is not null)
                {
                    var door = exit.Door;
                    var doorState = new DslDoorState
                    {
                        DoorId = door.Id,
                        State = "closed", // Default state - would need actual door state API
                        Locked = false, // Would need door locked state
                        Destroyed = false
                    };

                    if (!saveState.DoorStates.Any(d => d.DoorId == door.Id))
                        saveState.DoorStates.Add(doorState);
                }
            }
        }
    }

    private static void CollectExitDiscoveryStates(GameState gameState, DslCompleteGameState saveState)
    {
        // Collect hidden exit discovery state
        foreach (var location in gameState.Locations)
        {
            foreach (var (direction, exit) in location.Exits)
            {
                if (exit.IsHidden)
                {
                    var discovery = new DslExitDiscoveryState
                    {
                        LocationId = location.Id,
                        Direction = direction.ToString().ToLowerInvariant(),
                        Discovered = false, // Would need actual discovery tracking
                        DiscoveredAt = null
                    };

                    saveState.ExitDiscoveries.Add(discovery);
                }
            }
        }
    }

    private static void CollectNpcStates(GameState gameState, DslCompleteGameState saveState)
    {
        // Collect NPC states from game state
        // This is simplified - actual implementation would depend on NPC system
        // For now, we just provide the structure
    }

    private static void CollectQuestProgress(GameState gameState, DslCompleteGameState saveState)
    {
        // Collect active quest progress
        // This would pull from game state's quest tracking system
        // Simplified placeholder
    }

    private static void CollectStoryProgress(GameState gameState, DslCompleteGameState saveState)
    {
        saveState.StoryProgress = new DslStoryProgress
        {
            CurrentChapterId = "",
            CompletedChapters = [],
            DiscoveredBranches = [],
            BranchStates = []
        };
    }

    private static void CollectTimeState(GameState gameState, DslCompleteGameState saveState)
    {
        saveState.TimeState = new DslTimeState
        {
            CurrentTick = 0, // Would come from game state
            CurrentPhase = 0,
            InGameTime = DateTime.UtcNow
        };
    }

    private static void CollectRandomEventStates(GameState gameState, DslCompleteGameState saveState)
    {
        // Collect random event trigger states and cooldowns
        // Would pull from game's random event system
    }

    private static void CollectStatusEffects(GameState gameState, DslCompleteGameState saveState)
    {
        // Collect active poison/curse/buff effects
        // Would pull from game's status effect system if available
    }

    /// <summary>
    /// Restore game state from a save snapshot.
    /// </summary>
    public void RestoreSaveState(GameState gameState, DslCompleteGameState saveState)
    {
        ArgumentNullException.ThrowIfNull(gameState);
        ArgumentNullException.ThrowIfNull(saveState);

        // Validate save state
        var errors = saveState.Validate();
        if (errors.Count > 0)
            throw new InvalidOperationException($"Invalid save state: {string.Join(", ", errors)}");

        // Restore current location
        if (!string.IsNullOrEmpty(saveState.StartState.CurrentLocationId))
        {
            var location = gameState.Locations.FirstOrDefault(l => l.Id == saveState.StartState.CurrentLocationId);
            if (location is not null)
            {
                // Note: Would need to use game state's move method (name may vary)
                // This is a placeholder showing the intent
            }
        }

        // Restore door states
        RestoreDoorStates(gameState, saveState);

        // Restore NPC states (would need game engine support)
        RestoreNpcStates(gameState, saveState);

        // Restore quest progress (would need game engine support)
        RestoreQuestProgress(gameState, saveState);

        // Restore story progression (would need game engine support)
        RestoreStoryProgress(gameState, saveState);

        // Restore status effects (would need game engine support)
        RestoreStatusEffects(gameState, saveState);
    }

    private static void RestoreDoorStates(GameState gameState, DslCompleteGameState saveState)
    {
        // Restore door states from save data
        foreach (var doorState in saveState.DoorStates)
        {
            foreach (var location in gameState.Locations)
            {
                foreach (var exit in location.Exits.Values)
                {
                    if (exit.Door?.Id == doorState.DoorId)
                    {
                        // Would apply door state restoration here
                        // Requires door state mutation methods
                    }
                }
            }
        }
    }

    private static void RestoreNpcStates(GameState gameState, DslCompleteGameState saveState)
    {
        // Restore NPC locations and health
        // Would need NPC system integration
    }

    private static void RestoreQuestProgress(GameState gameState, DslCompleteGameState saveState)
    {
        // Restore quest states and progress
        // Would need quest system integration
    }

    private static void RestoreStoryProgress(GameState gameState, DslCompleteGameState saveState)
    {
        // Restore current chapter and branch states
        // Would need story system integration
    }

    private static void RestoreStatusEffects(GameState gameState, DslCompleteGameState saveState)
    {
        // Restore active status effects
        // Would need status effect system integration
    }
}

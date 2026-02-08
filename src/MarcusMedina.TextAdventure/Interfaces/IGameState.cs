// <copyright file="IGameState.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IGameState
{
    ILocation CurrentLocation { get; }
    bool Move(Direction direction);
    bool IsCurrentRoomId(string id);
    GameError LastMoveErrorCode { get; }
    RecipeBook RecipeBook { get; }
    IInventory Inventory { get; }
    IStats Stats { get; }
    IEventSystem Events { get; }
    ICombatSystem CombatSystem { get; }
    ITimeSystem TimeSystem { get; }
    IFactionSystem Factions { get; }
    IRandomEventPool RandomEvents { get; }
    IPathfinder Pathfinder { get; }
    ILocationDiscoverySystem LocationDiscovery { get; }
    IForeshadowingSystem Foreshadowing { get; }
    INarrativeVoiceSystem NarrativeVoice { get; }
    IAgencyTracker Agency { get; }
    IDramaticIronySystem DramaticIrony { get; }
    IFlashbackSystem Flashbacks { get; }
    IWorldState WorldState { get; }
    ISaveSystem SaveSystem { get; }
    /// <summary>Quest log for tracking active and completed quests.</summary>
    IQuestLog Quests { get; }
    StoryState Story { get; }
    bool ShowItemsListOnlyWhenThereAreActuallyThingsToInteractWith { get; set; }
    bool ShowDirectionsWhenThereAreDirectionsVisibleOnly { get; set; }
    /// <summary>Enable fuzzy matching for commands and targets.</summary>
    bool EnableFuzzyMatching { get; set; }
    /// <summary>Maximum edit distance for fuzzy matching.</summary>
    int FuzzyMaxDistance { get; set; }
    IReadOnlyCollection<ILocation> Locations { get; }
}

// <copyright file="IGameState.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public interface IGameState
{
    ICombatSystem CombatSystem { get; }
    ILocation CurrentLocation { get; }

    /// <summary>Enable fuzzy matching for commands and targets.</summary>
    bool EnableFuzzyMatching { get; set; }

    IEventSystem Events { get; }

    IFactionSystem Factions { get; }

    /// <summary>Maximum edit distance for fuzzy matching.</summary>
    int FuzzyMaxDistance { get; set; }

    IInventory Inventory { get; }

    GameError LastMoveErrorCode { get; }

    ILocationDiscoverySystem LocationDiscovery { get; }

    IReadOnlyCollection<ILocation> Locations { get; }

    /// <summary>Quest log for tracking active and completed quests.</summary>
    IQuestLog Quests { get; }

    IRandomEventPool RandomEvents { get; }

    RecipeBook RecipeBook { get; }

    ISaveSystem SaveSystem { get; }

    bool ShowDirectionsWhenThereAreDirectionsVisibleOnly { get; set; }

    bool ShowItemsListOnlyWhenThereAreActuallyThingsToInteractWith { get; set; }

    IStats Stats { get; }

    ITimeSystem TimeSystem { get; }

    IWorldState WorldState { get; }

    bool IsCurrentRoomId(string id);

    bool Move(Direction direction);
}
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
    ILocationDiscoverySystem LocationDiscovery { get; }
    IWorldState WorldState { get; }
    ISaveSystem SaveSystem { get; }
    IReadOnlyCollection<ILocation> Locations { get; }
}

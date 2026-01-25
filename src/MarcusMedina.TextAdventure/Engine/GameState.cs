// <copyright file="GameState.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public class GameState : IGameState
{
    private readonly Dictionary<string, ILocation> _locations = new(StringComparer.OrdinalIgnoreCase);

    public ILocation CurrentLocation { get; private set; }
    public string? LastMoveError { get; private set; }
    public GameError LastMoveErrorCode { get; private set; }
    public IStats Stats { get; }
    public IInventory Inventory { get; }
    public RecipeBook RecipeBook { get; }
    public IEventSystem Events { get; }
    public ICombatSystem CombatSystem { get; }
    public IWorldState WorldState { get; }
    public ISaveSystem SaveSystem { get; }
    public IReadOnlyCollection<ILocation> Locations => _locations.Values;

    public GameState(
        ILocation startLocation,
        IStats? stats = null,
        IInventory? inventory = null,
        RecipeBook? recipeBook = null,
        IEventSystem? eventSystem = null,
        ICombatSystem? combatSystem = null,
        IWorldState? worldState = null,
        ISaveSystem? saveSystem = null,
        IEnumerable<ILocation>? worldLocations = null)
    {
        ArgumentNullException.ThrowIfNull(startLocation);
        CurrentLocation = startLocation;
        Stats = stats ?? new Stats(100);
        Inventory = inventory ?? new Inventory();
        RecipeBook = recipeBook ?? new RecipeBook();
        Events = eventSystem ?? new EventSystem();
        CombatSystem = combatSystem ?? new TurnBasedCombat();
        WorldState = worldState ?? new WorldState();
        SaveSystem = saveSystem ?? new JsonSaveSystem();

        RegisterLocations(worldLocations ?? new[] { startLocation });
    }

    public void RegisterLocations(IEnumerable<ILocation> locations)
    {
        if (locations == null) return;
        foreach (var location in locations)
        {
            if (location == null) continue;
            _locations[location.Id] = location;
        }
    }

    public GameMemento CreateMemento()
    {
        var flags = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        var counters = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var relationships = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var timeline = new List<string>();

        if (WorldState is WorldState worldState)
        {
            flags = new Dictionary<string, bool>(worldState.GetFlagsSnapshot(), StringComparer.OrdinalIgnoreCase);
            counters = new Dictionary<string, int>(worldState.GetCountersSnapshot(), StringComparer.OrdinalIgnoreCase);
            relationships = new Dictionary<string, int>(worldState.GetRelationshipsSnapshot(), StringComparer.OrdinalIgnoreCase);
            timeline = worldState.Timeline.ToList();
        }

        return new GameMemento(
            CurrentLocation.Id,
            Inventory.Items.Select(i => i.Id).ToList(),
            Stats.Health,
            Stats.MaxHealth,
            flags,
            counters,
            relationships,
            timeline);
    }

    public void ApplyMemento(GameMemento memento)
    {
        ArgumentNullException.ThrowIfNull(memento);

        if (_locations.TryGetValue(memento.CurrentLocationId, out var location))
        {
            CurrentLocation = location;
        }

        Stats.SetMaxHealth(memento.MaxHealth);
        Stats.SetHealth(memento.Health);

        Inventory.Clear();
        var allLocations = _locations.Values.ToList();
        foreach (var itemId in memento.InventoryItemIds)
        {
            var item = FindItemById(itemId, allLocations);
            if (item == null) continue;
            var itemLocation = allLocations.FirstOrDefault(l => l.Items.Contains(item));
            itemLocation?.RemoveItem(item);
            Inventory.Add(item);
        }

        if (WorldState is WorldState worldState)
        {
            worldState.Apply(memento.Flags, memento.Counters, memento.Relationships, memento.Timeline);
        }
    }

    private static IItem? FindItemById(string id, IEnumerable<ILocation> locations)
    {
        foreach (var location in locations)
        {
            var item = location.Items.FirstOrDefault(i => i.Id.TextCompare(id));
            if (item != null) return item;
        }
        return null;
    }

    public bool Move(Direction direction)
    {
        LastMoveError = null;
        LastMoveErrorCode = GameError.None;
        var exit = CurrentLocation.GetExit(direction);

        if (exit == null)
        {
            LastMoveError = Language.CantGoThatWay;
            LastMoveErrorCode = GameError.NoExitInDirection;
            return false;
        }

        if (!exit.IsPassable)
        {
            if (exit.Door?.State == DoorState.Locked)
            {
                LastMoveError = Language.DoorLocked(exit.Door.Name);
                LastMoveErrorCode = GameError.DoorIsLocked;
            }
            else
            {
                LastMoveError = Language.DoorClosed(exit.Door!.Name);
                LastMoveErrorCode = GameError.DoorIsClosed;
            }
            return false;
        }

        var previousLocation = CurrentLocation;
        Events.Publish(new GameEvent(GameEventType.ExitLocation, this, previousLocation));
        CurrentLocation = exit.Target;
        Events.Publish(new GameEvent(GameEventType.EnterLocation, this, CurrentLocation));
        return true;
    }

    public bool IsCurrentRoomId(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;
        return CurrentLocation.Id.TextCompare(id);
    }
}

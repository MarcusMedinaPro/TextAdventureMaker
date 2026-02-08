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
    public ITimeSystem TimeSystem { get; private set; }
    public IFactionSystem Factions { get; private set; }
    public IRandomEventPool RandomEvents { get; private set; }
    public IPathfinder Pathfinder { get; private set; }
    public ILocationDiscoverySystem LocationDiscovery { get; private set; }
    public IForeshadowingSystem Foreshadowing { get; private set; }
    public INarrativeVoiceSystem NarrativeVoice { get; private set; }
    public IAgencyTracker Agency { get; private set; }
    public IDramaticIronySystem DramaticIrony { get; private set; }
    public IWorldState WorldState { get; }
    public ISaveSystem SaveSystem { get; }
    public IQuestLog Quests { get; }
    public StoryState Story { get; }
    private readonly NpcTriggerSystem _npcTriggers = new();
    public bool ShowItemsListOnlyWhenThereAreActuallyThingsToInteractWith { get; set; }
    public bool ShowDirectionsWhenThereAreDirectionsVisibleOnly { get; set; }
    /// <summary>Enable fuzzy matching for commands and targets.</summary>
    public bool EnableFuzzyMatching { get; set; }
    /// <summary>Maximum edit distance for fuzzy matching.</summary>
    public int FuzzyMaxDistance { get; set; } = 1;
    public IReadOnlyCollection<ILocation> Locations => _locations.Values;

    public GameState(
        ILocation startLocation,
        IStats? stats = null,
        IInventory? inventory = null,
        RecipeBook? recipeBook = null,
        IEventSystem? eventSystem = null,
        ICombatSystem? combatSystem = null,
        ITimeSystem? timeSystem = null,
        IFactionSystem? factionSystem = null,
        IRandomEventPool? randomEventPool = null,
        IPathfinder? pathfinder = null,
        ILocationDiscoverySystem? locationDiscovery = null,
        IForeshadowingSystem? foreshadowingSystem = null,
        INarrativeVoiceSystem? narrativeVoiceSystem = null,
        IAgencyTracker? agencyTracker = null,
        IDramaticIronySystem? dramaticIronySystem = null,
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
        TimeSystem = timeSystem ?? new TimeSystem();
        Factions = factionSystem ?? new FactionSystem();
        RandomEvents = randomEventPool ?? new RandomEventPool();
        Pathfinder = pathfinder ?? new AStarPathfinder();
        LocationDiscovery = locationDiscovery ?? new LocationDiscoverySystem();
        Foreshadowing = foreshadowingSystem ?? new ForeshadowingSystem();
        NarrativeVoice = narrativeVoiceSystem ?? new NarrativeVoiceSystem();
        Agency = agencyTracker ?? new AgencyTracker();
        DramaticIrony = dramaticIronySystem ?? new DramaticIronySystem();
        WorldState = worldState ?? new WorldState();
        SaveSystem = saveSystem ?? new JsonSaveSystem();
        Quests = new QuestLog();
        Story = new StoryState();

        RegisterLocations(worldLocations ?? new[] { startLocation });
        if (LocationDiscovery is LocationDiscoverySystem discovery)
        {
            discovery.Attach(this, Events);
        }
        _npcTriggers.Attach(this, Events);
    }

    public void RegisterLocations(IEnumerable<ILocation> locations)
    {
        if (locations == null)
        {
            return;
        }

        foreach (ILocation location in locations)
        {
            if (location == null)
            {
                continue;
            }

            _locations[location.Id] = location;
        }
    }

    public void SetTimeSystem(ITimeSystem timeSystem)
    {
        if (timeSystem == null)
        {
            return;
        }

        TimeSystem = timeSystem;
    }

    public void SetFactionSystem(IFactionSystem factionSystem)
    {
        if (factionSystem == null)
        {
            return;
        }

        Factions = factionSystem;
    }

    public void SetRandomEventPool(IRandomEventPool randomEventPool)
    {
        if (randomEventPool == null)
        {
            return;
        }

        RandomEvents = randomEventPool;
    }

    public void SetPathfinder(IPathfinder pathfinder)
    {
        if (pathfinder == null)
        {
            return;
        }

        Pathfinder = pathfinder;
    }

    public void TickNpcTriggers()
    {
        _npcTriggers.Tick(this);
    }

    public void SetLocationDiscoverySystem(ILocationDiscoverySystem locationDiscovery)
    {
        if (locationDiscovery == null)
        {
            return;
        }

        LocationDiscovery = locationDiscovery;
        if (locationDiscovery is LocationDiscoverySystem discovery)
        {
            discovery.Attach(this, Events);
        }
    }

    public void SetForeshadowingSystem(IForeshadowingSystem foreshadowingSystem)
    {
        if (foreshadowingSystem == null)
        {
            return;
        }

        Foreshadowing = foreshadowingSystem;
    }

    public void SetNarrativeVoiceSystem(INarrativeVoiceSystem narrativeVoiceSystem)
    {
        if (narrativeVoiceSystem == null)
        {
            return;
        }

        NarrativeVoice = narrativeVoiceSystem;
    }

    public void SetAgencyTracker(IAgencyTracker agencyTracker)
    {
        if (agencyTracker == null)
        {
            return;
        }

        Agency = agencyTracker;
    }

    public void SetDramaticIronySystem(IDramaticIronySystem dramaticIronySystem)
    {
        if (dramaticIronySystem == null)
        {
            return;
        }

        DramaticIrony = dramaticIronySystem;
    }

    public GameMemento CreateMemento()
    {
        Dictionary<string, bool> flags = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, int> counters = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, int> relationships = new(StringComparer.OrdinalIgnoreCase);
        List<string> timeline = [];

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

        if (_locations.TryGetValue(memento.CurrentLocationId, out ILocation? location))
        {
            CurrentLocation = location;
        }

        Stats.SetMaxHealth(memento.MaxHealth);
        Stats.SetHealth(memento.Health);

        Inventory.Clear();
        List<ILocation> allLocations = _locations.Values.ToList();
        foreach (string itemId in memento.InventoryItemIds)
        {
            IItem? item = FindItemById(itemId, allLocations);
            if (item == null)
            {
                continue;
            }

            ILocation? itemLocation = allLocations.FirstOrDefault(l => l.Items.Contains(item));
            _ = (itemLocation?.RemoveItem(item));
            _ = Inventory.Add(item);
        }

        if (WorldState is WorldState worldState)
        {
            worldState.Apply(memento.Flags, memento.Counters, memento.Relationships, memento.Timeline);
        }
    }

    private static IItem? FindItemById(string id, IEnumerable<ILocation> locations)
    {
        foreach (ILocation location in locations)
        {
            IItem? item = location.Items.FirstOrDefault(i => i.Id.TextCompare(id));
            if (item != null)
            {
                return item;
            }
        }

        return null;
    }

    public bool Move(Direction direction)
    {
        LastMoveError = null;
        LastMoveErrorCode = GameError.None;
        Exit? exit = CurrentLocation.GetExit(direction);

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

        ILocation previousLocation = CurrentLocation;
        Events.Publish(new GameEvent(GameEventType.ExitLocation, this, previousLocation));
        CurrentLocation = exit.Target;
        Events.Publish(new GameEvent(GameEventType.EnterLocation, this, CurrentLocation));
        _ = CurrentLocation.DiscoverHiddenExits(this);
        return true;
    }

    public bool IsCurrentRoomId(string id)
    {
        return !string.IsNullOrWhiteSpace(id) && CurrentLocation.Id.TextCompare(id);
    }
}

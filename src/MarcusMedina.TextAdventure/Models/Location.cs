// <copyright file="Location.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Location : ILocation
{
    public string Id { get; }
    public string Name { get; }
    private string _description = "";
    private readonly Dictionary<Direction, Exit> _exits = [];
    private readonly List<IItem> _items = [];
    private readonly List<INpc> _npcs = [];
    private DynamicDescription? _dynamicDescription;
    private LayeredDescription? _layeredDescription;
    private int _visitCount;
    private readonly List<FlashbackTrigger> _flashbackTriggers = [];
    private readonly List<TimedSpawn> _timedSpawns = [];
    private LocationTransform? _transform;

    public IReadOnlyDictionary<Direction, Exit> Exits => _exits;
    public IReadOnlyList<IItem> Items => _items;
    public IReadOnlyList<INpc> Npcs => _npcs;
    public IReadOnlyList<FlashbackTrigger> FlashbackTriggers => _flashbackTriggers;
    public IReadOnlyList<TimedSpawn> TimedSpawns => _timedSpawns;
    public LocationTransform? Transform => _transform;

    public Location(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
        Name = id;
    }

    public Location(string id, string description) : this(id)
    {
        _description = description ?? "";
    }

    public Location(string id, string name, string description) : this(id, description)
    {
        Name = name;
    }

    public Location Description(string text)
    {
        _description = text;
        return this;
    }

    public string GetDescription()
    {
        return _description;
    }

    public string GetDescription(IGameState state)
    {
        if (state == null)
        {
            return GetDescription();
        }

        bool firstVisit = _visitCount == 0;
        string text = _layeredDescription != null
            ? _layeredDescription.Resolve(state, _visitCount)
            : _dynamicDescription?.Resolve(state, firstVisit) ?? GetDescription();

        _visitCount++;
        return text;
    }

    public Location SetDynamicDescription(DynamicDescription description)
    {
        _dynamicDescription = description;
        return this;
    }

    public LayeredDescription SetLayeredDescription()
    {
        _layeredDescription = new LayeredDescription();
        return _layeredDescription;
    }

    public Location SetLayeredDescription(LayeredDescription description)
    {
        _layeredDescription = description;
        return this;
    }

    public FlashbackTrigger AddFlashbackTrigger(string memoryId)
    {
        FlashbackTrigger trigger = new(memoryId);
        _flashbackTriggers.Add(trigger);
        return trigger;
    }

    public TimedSpawn AddTimedSpawn(string itemId)
    {
        TimedSpawn spawn = new(itemId);
        _timedSpawns.Add(spawn);
        return spawn;
    }

    public LocationTransformBuilder TransformsInto(string locationId)
    {
        _transform = new LocationTransform(locationId);
        return new LocationTransformBuilder(_transform);
    }

    public Location AddExit(Direction direction, ILocation target, bool oneWay = false)
    {
        _exits[direction] = new Exit(target, null, oneWay);

        if (!oneWay && target is Location targetLoc)
        {
            Direction opposite = DirectionHelper.GetOpposite(direction);
            targetLoc._exits[opposite] = new Exit(this);
        }

        return this;
    }

    public Location AddExit(Direction direction, ILocation target, IDoor door, bool oneWay = false)
    {
        _exits[direction] = new Exit(target, door, oneWay);

        if (!oneWay && target is Location targetLoc)
        {
            Direction opposite = DirectionHelper.GetOpposite(direction);
            targetLoc._exits[opposite] = new Exit(this, door); // Same door from other side
        }

        return this;
    }

    public Location AddHiddenExit(Direction direction, ILocation target, Func<IGameState, bool>? discoverCondition = null, bool oneWay = false)
    {
        Exit exit = new Exit(target, null, oneWay).MarkHidden(discoverCondition);
        _exits[direction] = exit;

        if (!oneWay && target is Location targetLoc)
        {
            Direction opposite = DirectionHelper.GetOpposite(direction);
            Exit backExit = new Exit(this).MarkHidden(discoverCondition);
            targetLoc._exits[opposite] = backExit;
        }

        return this;
    }

    public Exit? GetExit(Direction direction)
    {
        if (!_exits.TryGetValue(direction, out Exit? exit))
        {
            return null;
        }

        return exit.IsVisible ? exit : null;
    }

    public bool DiscoverHiddenExits(IGameState state)
    {
        bool discoveredAny = false;
        foreach (Exit exit in _exits.Values)
        {
            if (exit.TryDiscover(state))
            {
                discoveredAny = true;
            }
        }

        return discoveredAny;
    }

    public void AddItem(IItem item)
    {
        _items.Add(item);
    }

    public bool RemoveItem(IItem item)
    {
        return _items.Remove(item);
    }

    public IItem? FindItem(string name)
    {
        return string.IsNullOrWhiteSpace(name) ? null : _items.FirstOrDefault(i => i.Matches(name));
    }

    public void AddNpc(INpc npc)
    {
        _npcs.Add(npc);
    }

    public bool RemoveNpc(INpc npc)
    {
        return _npcs.Remove(npc);
    }

    public INpc? FindNpc(string name)
    {
        return string.IsNullOrWhiteSpace(name) ? null : _npcs.FirstOrDefault(n => n.Name.TextCompare(name));
    }

    public static implicit operator Location(string id)
    {
        return new(id);
    }

    public static implicit operator Location((string id, string description) data)
    {
        return new(data.id, data.description);
    }
}

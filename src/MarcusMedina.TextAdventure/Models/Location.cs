// <copyright file="Location.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;

public class Location : ILocation
{
    public string Id { get; }
    private string _description = "";
    private readonly Dictionary<Direction, Exit> _exits = [];
    private readonly List<IItem> _items = [];
    private readonly List<INpc> _npcs = [];

    public IReadOnlyDictionary<Direction, Exit> Exits => _exits;
    public IReadOnlyList<IItem> Items => _items;
    public IReadOnlyList<INpc> Npcs => _npcs;

    public Location(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
    }

    public Location(string id, string description) : this(id) => _description = description ?? "";

    public Location Description(string text)
    {
        _description = text;
        return this;
    }

    public string GetDescription() => _description;

    public Location AddExit(Direction direction, ILocation target, bool oneWay = false)
    {
        _exits[direction] = new Exit(target, null, oneWay);

        if (!oneWay && target is Location targetLoc)
        {
            var opposite = DirectionHelper.GetOpposite(direction);
            targetLoc._exits[opposite] = new Exit(this);
        }

        return this;
    }

    public Location AddExit(Direction direction, ILocation target, IDoor door, bool oneWay = false)
    {
        _exits[direction] = new Exit(target, door, oneWay);

        if (!oneWay && target is Location targetLoc)
        {
            var opposite = DirectionHelper.GetOpposite(direction);
            targetLoc._exits[opposite] = new Exit(this, door); // Same door from other side
        }

        return this;
    }

    public Exit? GetExit(Direction direction) => _exits.TryGetValue(direction, out var exit) ? exit : null;

    public void AddItem(IItem item) => _items.Add(item);

    public bool RemoveItem(IItem item) => _items.Remove(item);

    public IItem? FindItem(string name) => string.IsNullOrWhiteSpace(name) ? null : _items.FirstOrDefault(i => i.Matches(name));

    public void AddNpc(INpc npc) => _npcs.Add(npc);

    public bool RemoveNpc(INpc npc) => _npcs.Remove(npc);

    public INpc? FindNpc(string name) => string.IsNullOrWhiteSpace(name) ? null : _npcs.FirstOrDefault(n => n.Name.TextCompare(name));

    public static implicit operator Location(string id) => new(id);
    public static implicit operator Location((string id, string description) data) =>
        new(data.id, data.description);
}

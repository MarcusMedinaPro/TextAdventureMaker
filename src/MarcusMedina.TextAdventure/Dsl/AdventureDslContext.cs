// <copyright file="AdventureDslContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Dsl;

public sealed class AdventureDslContext
{
    private readonly Dictionary<string, Location> _locations = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Item> _items = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Key> _keys = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Door> _doors = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _metadata = new(StringComparer.OrdinalIgnoreCase);

    internal List<PendingExit> PendingExits { get; } = [];
    internal List<PendingDoorKey> PendingDoorKeys { get; } = [];

    public Location? CurrentLocation { get; private set; }
    public IReadOnlyDictionary<string, Location> Locations => _locations;
    public IReadOnlyDictionary<string, Item> Items => _items;
    public IReadOnlyDictionary<string, Key> Keys => _keys;
    public IReadOnlyDictionary<string, Door> Doors => _doors;
    public IReadOnlyDictionary<string, string> Metadata => _metadata;
    public string? StartLocationId { get; set; }

    public void SetMetadata(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        _metadata[key.Trim()] = value ?? "";
    }

    public string? GetMetadata(string key)
    {
        return string.IsNullOrWhiteSpace(key) ? null : _metadata.TryGetValue(key.Trim(), out string? value) ? value : null;
    }

    public Location GetOrCreateLocation(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Location id is required.", nameof(id));
        }

        if (_locations.TryGetValue(id, out Location? location))
        {
            return location;
        }

        location = new Location(id);
        _locations[id] = location;
        return location;
    }

    public void SetCurrentLocation(string id, string? description = null)
    {
        Location location = GetOrCreateLocation(id);
        if (!string.IsNullOrWhiteSpace(description))
        {
            _ = location.Description(description);
        }

        CurrentLocation = location;
    }

    public Item GetOrCreateItem(string id, string name, string? description = null)
    {
        if (_items.TryGetValue(id, out Item? item))
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                _ = item.SetDescription(description);
            }

            return item;
        }

        item = string.IsNullOrWhiteSpace(description)
            ? new Item(id, name)
            : new Item(id, name, description);

        _items[id] = item;
        return item;
    }

    public Key GetOrCreateKey(string id, string name, string? description = null)
    {
        if (_keys.TryGetValue(id, out Key? key))
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                _ = key.SetDescription(description);
            }

            return key;
        }

        key = string.IsNullOrWhiteSpace(description)
            ? new Key(id, name)
            : new Key(id, name, description);

        _keys[id] = key;
        return key;
    }

    public Door GetOrCreateDoor(string id, string name, string? description = null)
    {
        if (_doors.TryGetValue(id, out Door? door))
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                _ = door.Description(description);
            }

            return door;
        }

        door = string.IsNullOrWhiteSpace(description)
            ? new Door(id, name)
            : new Door(id, name, description);

        _doors[id] = door;
        return door;
    }

    public void RequireCurrentLocation()
    {
        if (CurrentLocation == null)
        {
            throw new InvalidOperationException("No current location set for DSL entries.");
        }
    }
}

internal sealed record PendingExit(
    string FromId,
    string TargetId,
    Direction Direction,
    string? DoorId,
    bool IsOneWay);

internal sealed record PendingDoorKey(
    string DoorId,
    string KeyId);

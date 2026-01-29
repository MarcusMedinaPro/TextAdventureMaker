// <copyright file="LocationList.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using System.Linq;
using MarcusMedina.TextAdventure.Extensions;

public sealed class LocationList
{
    private readonly Dictionary<string, Location> _locations = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<Location> Items => _locations.Values;

    public Location Add(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Add(new Location(name.ToId(), name));
    }

    public Location Add(string name, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return Add(new Location(name.ToId(), description ?? ""));
    }

    public Location Add(Location location)
    {
        ArgumentNullException.ThrowIfNull(location);
        _locations[location.Id] = location;
        return location;
    }

    public LocationList AddMany(params string[] names)
    {
        if (names == null)
            return this;
        foreach (var name in names)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Add(name);
            }
        }

        return this;
    }

    public LocationList AddMany(IEnumerable<string> names)
    {
        if (names == null)
            return this;
        foreach (var name in names)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _ = Add(name);
            }
        }

        return this;
    }

    public Location? Find(string token) => string.IsNullOrWhiteSpace(token)
            ? null
            : _locations.TryGetValue(token, out var location) ? location : _locations.Values.FirstOrDefault(l => l.Id.TextCompare(token));

    public Location Get(string token)
    {
        var location = Find(token);
        return location ?? throw new KeyNotFoundException($"No location found for '{token}'.");
    }

    public bool TryGet(string token, out Location location)
    {
        location = Find(token) ?? null!;
        return location != null;
    }

    public bool Remove(string token)
    {
        var location = Find(token);
        return location != null && _locations.Remove(location.Id);
    }

    public void Clear() => _locations.Clear();

    public Location this[string token] => Get(token);
    public Location Call(string token) => Get(token);
}

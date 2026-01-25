// <copyright file="DslAdventure.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Dsl;

public sealed class DslAdventure
{
    public GameState State { get; }
    public IReadOnlyDictionary<string, Location> Locations { get; }
    public IReadOnlyDictionary<string, Item> Items { get; }
    public IReadOnlyDictionary<string, Key> Keys { get; }
    public IReadOnlyDictionary<string, Door> Doors { get; }
    public IReadOnlyDictionary<string, string> Metadata { get; }

    public string? WorldName => GetMetadata("world");
    public string? Goal => GetMetadata("goal");

    public DslAdventure(
        GameState state,
        IReadOnlyDictionary<string, Location> locations,
        IReadOnlyDictionary<string, Item> items,
        IReadOnlyDictionary<string, Key> keys,
        IReadOnlyDictionary<string, Door> doors,
        IReadOnlyDictionary<string, string> metadata)
    {
        State = state ?? throw new ArgumentNullException(nameof(state));
        Locations = locations ?? throw new ArgumentNullException(nameof(locations));
        Items = items ?? throw new ArgumentNullException(nameof(items));
        Keys = keys ?? throw new ArgumentNullException(nameof(keys));
        Doors = doors ?? throw new ArgumentNullException(nameof(doors));
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
    }

    public string? GetMetadata(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;
        return Metadata.TryGetValue(key, out var value) ? value : null;
    }
}

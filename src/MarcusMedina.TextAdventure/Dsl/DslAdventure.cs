// <copyright file="DslAdventure.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Dsl;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;

public sealed class DslAdventure(
    GameState state,
    IReadOnlyDictionary<string, Location> locations,
    IReadOnlyDictionary<string, Item> items,
    IReadOnlyDictionary<string, Key> keys,
    IReadOnlyDictionary<string, Door> doors,
    IReadOnlyDictionary<string, string> metadata)
{
    public GameState State { get; } = state ?? throw new ArgumentNullException(nameof(state));
    public IReadOnlyDictionary<string, Location> Locations { get; } = locations ?? throw new ArgumentNullException(nameof(locations));
    public IReadOnlyDictionary<string, Item> Items { get; } = items ?? throw new ArgumentNullException(nameof(items));
    public IReadOnlyDictionary<string, Key> Keys { get; } = keys ?? throw new ArgumentNullException(nameof(keys));
    public IReadOnlyDictionary<string, Door> Doors { get; } = doors ?? throw new ArgumentNullException(nameof(doors));
    public IReadOnlyDictionary<string, string> Metadata { get; } = metadata ?? throw new ArgumentNullException(nameof(metadata));

    public string? WorldName => GetMetadata("world");
    public string? Goal => GetMetadata("goal");

    public string? GetMetadata(string key) => string.IsNullOrWhiteSpace(key) ? null : Metadata.TryGetValue(key, out var value) ? value : null;
}

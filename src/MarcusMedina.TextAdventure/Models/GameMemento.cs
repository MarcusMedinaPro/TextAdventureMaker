// <copyright file="GameMemento.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class GameMemento : IMemento
{
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public string CurrentLocationId { get; init; } = "";
    public IReadOnlyList<string> InventoryItemIds { get; init; } = [];
    public int Health { get; init; }
    public int MaxHealth { get; init; }
    public IReadOnlyDictionary<string, bool> Flags { get; init; } = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, int> Counters { get; init; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyDictionary<string, int> Relationships { get; init; } = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<string> Timeline { get; init; } = [];

    public GameMemento()
    {
    }

    public GameMemento(
        string currentLocationId,
        IEnumerable<string> inventoryItemIds,
        int health,
        int maxHealth,
        IReadOnlyDictionary<string, bool> flags,
        IReadOnlyDictionary<string, int> counters,
        IReadOnlyDictionary<string, int> relationships,
        IReadOnlyList<string> timeline)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currentLocationId);
        CurrentLocationId = currentLocationId;
        InventoryItemIds = inventoryItemIds?.ToList() ?? [];
        Health = health;
        MaxHealth = maxHealth;
        Flags = flags ?? new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        Counters = counters ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        Relationships = relationships ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        Timeline = timeline ?? [];
        CreatedAt = DateTimeOffset.UtcNow;
    }
}

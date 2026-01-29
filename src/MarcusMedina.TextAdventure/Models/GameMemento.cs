// <copyright file="GameMemento.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public sealed class GameMemento : IMemento
{
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public string CurrentLocationId { get; set; } = "";
    public List<string> InventoryItemIds { get; set; } = [];
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public Dictionary<string, bool> Flags { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> Counters { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, int> Relationships { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<string> Timeline { get; set; } = [];

    public GameMemento()
    {
    }

    public GameMemento(
        string currentLocationId,
        IEnumerable<string> inventoryItemIds,
        int health,
        int maxHealth,
        Dictionary<string, bool> flags,
        Dictionary<string, int> counters,
        Dictionary<string, int> relationships,
        List<string> timeline)
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

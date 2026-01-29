// <copyright file="FactionSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Engine;

using MarcusMedina.TextAdventure.Interfaces;

public sealed class FactionSystem : IFactionSystem
{
    private readonly Dictionary<string, IFaction> _factions = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyDictionary<string, IFaction> Factions => _factions;

    public IFaction AddFaction(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Faction id cannot be empty.", nameof(id));
        }

        if (_factions.TryGetValue(id, out var existing))
        {
            return existing;
        }

        var faction = new Faction(id);
        _factions[id] = faction;
        return faction;
    }

    public IFaction? GetFaction(string id) => string.IsNullOrWhiteSpace(id) ? null : _factions.TryGetValue(id, out var faction) ? faction : null;

    public int GetReputation(string id) => GetFaction(id)?.Reputation ?? 0;

    public int ModifyReputation(string id, int amount, IGameState state)
    {
        var faction = AddFaction(id);
        return ((Faction)faction).ModifyReputation(amount, state);
    }
}

// <copyright file="Faction.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Engine;

using MarcusMedina.TextAdventure.Interfaces;

public sealed class Faction : IFaction
{
    private readonly HashSet<string> _npcIds = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<(int threshold, Action<IGameState> handler)> _thresholds = [];
    private readonly HashSet<int> _thresholdsFired = [];

    public string Id { get; }
    public int Reputation { get; private set; }
    public IReadOnlyCollection<string> NpcIds => _npcIds;

    public Faction(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
    }

    public IFaction WithNpcs(params string[] npcIds)
    {
        if (npcIds == null)
            return this;
        foreach (var npcId in npcIds)
        {
            if (!string.IsNullOrWhiteSpace(npcId))
            {
                _ = _npcIds.Add(npcId.Trim());
            }
        }

        return this;
    }

    public IFaction OnReputationThreshold(int threshold, Action<IGameState> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        _thresholds.Add((threshold, handler));
        return this;
    }

    public bool HasNpc(string npcId) => !string.IsNullOrWhiteSpace(npcId) && _npcIds.Contains(npcId);

    public int ModifyReputation(int amount, IGameState state)
    {
        var previous = Reputation;
        Reputation += amount;

        foreach (var (threshold, handler) in _thresholds)
        {
            if (_thresholdsFired.Contains(threshold))
                continue;

            var crossed = threshold >= 0
                ? previous < threshold && Reputation >= threshold
                : previous > threshold && Reputation <= threshold;

            if (crossed)
            {
                _ = _thresholdsFired.Add(threshold);
                handler(state);
            }
        }

        return Reputation;
    }
}

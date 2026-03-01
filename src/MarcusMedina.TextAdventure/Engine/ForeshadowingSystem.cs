// <copyright file="ForeshadowingSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class ForeshadowingSystem : IForeshadowingSystem
{
    private sealed class Seed
    {
        public HashSet<string> Links { get; } = new(StringComparer.OrdinalIgnoreCase);
        public bool Planted { get; set; }
        public bool Hinted { get; set; }
        public bool PaidOff { get; set; }
    }

    private readonly Dictionary<string, Seed> _seeds = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> Unpaid => _seeds
        .Where(seed => seed.Value.Planted && !seed.Value.PaidOff)
        .Select(seed => seed.Key)
        .ToArray();

    public IReadOnlyCollection<string> Unhinted => _seeds
        .Where(seed => seed.Value.Planted && !seed.Value.Hinted)
        .Select(seed => seed.Key)
        .ToArray();

    public void Plant(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return;
        }

        Seed seed = GetOrCreate(tag);
        seed.Planted = true;
    }

    public void Hint(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return;
        }

        Seed seed = GetOrCreate(tag);
        seed.Hinted = true;
    }

    public void Link(string tag, string linkId)
    {
        if (string.IsNullOrWhiteSpace(tag) || string.IsNullOrWhiteSpace(linkId))
        {
            return;
        }

        Seed seed = GetOrCreate(tag);
        seed.Links.Add(linkId);
    }

    public IReadOnlyCollection<string> TagsFor(string linkId)
    {
        if (string.IsNullOrWhiteSpace(linkId))
        {
            return Array.Empty<string>();
        }

        return _seeds
            .Where(seed => seed.Value.Links.Contains(linkId))
            .Select(seed => seed.Key)
            .ToArray();
    }

    public void Payoff(string tag, IGameState? state = null, Action<IGameState>? missedHintCallback = null)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return;
        }

        Seed seed = GetOrCreate(tag);
        seed.PaidOff = true;

        if (!seed.Hinted && missedHintCallback != null && state != null)
        {
            missedHintCallback(state);
        }
    }

    private Seed GetOrCreate(string tag)
    {
        if (!_seeds.TryGetValue(tag, out Seed? seed))
        {
            seed = new Seed();
            _seeds[tag] = seed;
        }

        return seed;
    }
}

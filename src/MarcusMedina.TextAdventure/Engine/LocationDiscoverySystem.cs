// <copyright file="LocationDiscoverySystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class LocationDiscoverySystem : ILocationDiscoverySystem
{
    private readonly HashSet<string> _discovered = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> DiscoveredLocations => _discovered;

    public void Attach(IGameState state, IEventSystem events)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(events);
        Discover(state.CurrentLocation.Id);
        events.Subscribe(GameEventType.EnterLocation, e =>
        {
            if (e.Location != null)
            {
                Discover(e.Location.Id);
            }
        });
    }

    public bool IsDiscovered(string locationId)
    {
        return !string.IsNullOrWhiteSpace(locationId) && _discovered.Contains(locationId);
    }

    public void Discover(string locationId)
    {
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return;
        }

        _ = _discovered.Add(locationId.Trim());
    }
}

// <copyright file="SpatialEventSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Manages events that propagate through connected rooms based on spatial proximity.
/// Example: A shout in one room propagates to adjacent and nearby rooms.
/// </summary>
public sealed class SpatialEventSystem
{
    /// <summary>
    /// Propagates an event from an origin location to all adjacent locations within range.
    /// </summary>
    public void PropagateEvent(string eventName, ILocation origin, int range = 1)
    {
        ArgumentNullException.ThrowIfNull(eventName);
        ArgumentNullException.ThrowIfNull(origin);

        var visited = new HashSet<ILocation> { origin };
        var queue = new Queue<(ILocation location, int distance)>();
        queue.Enqueue((origin, 0));

        while (queue.Count > 0)
        {
            var (currentLocation, distance) = queue.Dequeue();

            if (distance > range)
                continue;

            // TODO: Event propagation will be wired once ILocation supports events
            foreach (var exit in currentLocation.Exits.Values)
            {
                if (!visited.Contains(exit.Target))
                {
                    visited.Add(exit.Target);
                    queue.Enqueue((exit.Target, distance + 1));
                }
            }
        }
    }
}

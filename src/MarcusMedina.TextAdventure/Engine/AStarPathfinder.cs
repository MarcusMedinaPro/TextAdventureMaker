// <copyright file="AStarPathfinder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class AStarPathfinder : IPathfinder
{
    public IReadOnlyList<Direction> FindPath(ILocation start, ILocation goal)
    {
        ArgumentNullException.ThrowIfNull(start);
        ArgumentNullException.ThrowIfNull(goal);

        if (ReferenceEquals(start, goal))
        {
            return Array.Empty<Direction>();
        }

        Dictionary<ILocation, (ILocation? Prev, Direction? Dir)> visited = new();
        Queue<ILocation> queue = new();
        queue.Enqueue(start);
        visited[start] = (null, null);

        while (queue.Count > 0)
        {
            ILocation current = queue.Dequeue();
            if (ReferenceEquals(current, goal))
            {
                break;
            }

            foreach (KeyValuePair<Direction, Exit> entry in current.Exits)
            {
                ILocation next = entry.Value.Target;
                if (visited.ContainsKey(next))
                {
                    continue;
                }

                visited[next] = (current, entry.Key);
                queue.Enqueue(next);
            }
        }

        if (!visited.ContainsKey(goal))
        {
            return Array.Empty<Direction>();
        }

        List<Direction> path = [];
        ILocation? step = goal;
        while (step != null && !ReferenceEquals(step, start))
        {
            (ILocation? prev, Direction? dir) = visited[step];
            if (dir.HasValue)
            {
                path.Add(dir.Value);
            }

            step = prev;
        }

        path.Reverse();
        return path;
    }
}

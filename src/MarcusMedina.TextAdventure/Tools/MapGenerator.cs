// <copyright file="MapGenerator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;
using System.Text;

namespace MarcusMedina.TextAdventure.Tools;

public static class MapGenerator
{
    public static string Render(GameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        return Render(state.CurrentLocation, 10);
    }

    public static string Render(ILocation start, int maxDepth)
    {
        ArgumentNullException.ThrowIfNull(start);
        maxDepth = Math.Max(0, maxDepth);

        StringBuilder builder = new();
        HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
        Queue<(ILocation Location, int Depth)> queue = new();
        queue.Enqueue((start, 0));
        visited.Add(start.Id);

        _ = builder.AppendLine("=== MAP ===");
        while (queue.Count > 0)
        {
            (ILocation location, int depth) = queue.Dequeue();
            _ = builder.AppendLine(location.Id);

            foreach (KeyValuePair<Enums.Direction, Models.Exit> exit in location.Exits)
            {
                _ = builder.AppendLine($"  {exit.Key}: {exit.Value.Target.Id}");
                if (depth + 1 <= maxDepth && visited.Add(exit.Value.Target.Id))
                {
                    queue.Enqueue((exit.Value.Target, depth + 1));
                }
            }

            _ = builder.AppendLine();
        }

        return builder.ToString().TrimEnd();
    }
}

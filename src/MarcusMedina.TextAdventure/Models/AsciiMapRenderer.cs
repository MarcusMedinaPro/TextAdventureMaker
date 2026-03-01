// <copyright file="AsciiMapRenderer.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using System.Text;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Renders map data as ASCII art for terminal display.
/// </summary>
public sealed class AsciiMapRenderer
{
    private const char RoomCharacter = '█';
    private const char CurrentRoomCharacter = '@';
    private const char UnvisitedCharacter = '?';
    private const char HorizontalPath = '─';
    private const char VerticalPath = '│';
    private const char DoorCharacter = '▒';
    private const char EmptySpace = ' ';

    /// <summary>
    /// Renders map data as an ASCII grid.
    /// </summary>
    public string Render(MapData mapData, IGameState state, MapOptions options)
    {
        ArgumentNullException.ThrowIfNull(mapData);
        ArgumentNullException.ThrowIfNull(state);
        options ??= new MapOptions();

        if (mapData.Positions.Count == 0)
            return "No map data to render.";

        var grid = CreateGrid(mapData, options);
        RenderLocations(grid, mapData, state, options);
        RenderConnections(grid, mapData, state, options);

        return GridToString(grid);
    }

    private static char[,] CreateGrid(MapData mapData, MapOptions options)
    {
        var maxX = mapData.Positions.Values.Max(p => p.X);
        var maxY = mapData.Positions.Values.Max(p => p.Y);

        var width = Math.Min(maxX + 2, options.MaxWidth);
        var height = Math.Min(maxY + 2, options.MaxHeight);

        var grid = new char[height, width];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                grid[y, x] = EmptySpace;
            }
        }

        return grid;
    }

    private void RenderLocations(char[,] grid, MapData mapData, IGameState state, MapOptions options)
    {
        var visitedLocations = GetVisitedLocations(state);

        foreach (var (location, position) in mapData.Positions)
        {
            if (!IsInBounds(position, grid))
                continue;

            var displayChar = location == state.CurrentLocation
                ? CurrentRoomCharacter
                : !visitedLocations.Contains(location) && !options.ShowUnvisited
                    ? UnvisitedCharacter
                    : RoomCharacter;

            grid[position.Y, position.X] = displayChar;
        }
    }

    private static List<ILocation> GetVisitedLocations(IGameState state)
    {
        var visited = new HashSet<ILocation> { state.CurrentLocation };
        var queue = new Queue<ILocation>();
        queue.Enqueue(state.CurrentLocation);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var exit in current.Exits.Values)
            {
                if (visited.Add(exit.Target))
                {
                    queue.Enqueue(exit.Target);
                }
            }
        }

        return visited.ToList();
    }

    private void RenderConnections(char[,] grid, MapData mapData, IGameState state, MapOptions options)
    {
        foreach (var connection in mapData.Connections)
        {
            if (!mapData.Positions.TryGetValue(connection.From, out var fromPos))
                continue;

            var pathChar = GetPathCharacter(connection.Direction, connection.From);
            var pathPos = GetPathPosition(fromPos, connection.Direction);

            if (IsInBounds(pathPos, grid))
            {
                grid[pathPos.Y, pathPos.X] = pathChar;
            }
        }
    }

    private char GetPathCharacter(Direction direction, ILocation location)
    {
        var door = location.Exits.TryGetValue(direction, out var exit) ? exit.Door : null;
        var baseChar = direction is Direction.North or Direction.South or Direction.Up or Direction.Down
            ? VerticalPath
            : HorizontalPath;

        return door is not null && door.State != DoorState.Open ? DoorCharacter : baseChar;
    }

    private static Point GetPathPosition(Point fromPosition, Direction direction) =>
        direction switch
        {
            Direction.North => fromPosition with { Y = fromPosition.Y - 1 },
            Direction.South => fromPosition with { Y = fromPosition.Y + 1 },
            Direction.East => fromPosition with { X = fromPosition.X + 1 },
            Direction.West => fromPosition with { X = fromPosition.X - 1 },
            _ => fromPosition
        };

    private static bool IsInBounds(Point position, char[,] grid) =>
        position.X >= 0 && position.X < grid.GetLength(1) &&
        position.Y >= 0 && position.Y < grid.GetLength(0);

    private static string GridToString(char[,] grid)
    {
        var sb = new StringBuilder();
        var height = grid.GetLength(0);
        var width = grid.GetLength(1);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                sb.Append(grid[y, x]);
            }

            if (y < height - 1)
                sb.AppendLine();
        }

        return sb.ToString();
    }
}

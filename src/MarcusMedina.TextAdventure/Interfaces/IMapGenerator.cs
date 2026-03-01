// <copyright file="IMapGenerator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Generates ASCII maps and procedural world data.
/// </summary>
public interface IMapGenerator
{
    /// <summary>
    /// Generates an ASCII representation of the world.
    /// </summary>
    string GenerateAsciiMap(IGameState state, MapOptions? options = null);

    /// <summary>
    /// Generates structured map data for procedural world generation.
    /// </summary>
    MapData GenerateMapData(IGameState state);
}

/// <summary>
/// Configuration options for map generation.
/// </summary>
public record MapOptions(
    bool ShowUnvisited = false,
    bool ShowItems = false,
    bool ShowNpcs = false,
    int MaxWidth = 80,
    int MaxHeight = 24
);

/// <summary>
/// Structured representation of map layout and connections.
/// </summary>
public record MapData(
    Dictionary<ILocation, Point> Positions,
    List<MapConnection> Connections
);

/// <summary>
/// A coordinate point on the map grid.
/// </summary>
public record Point(int X, int Y);

/// <summary>
/// Represents a connection between two rooms.
/// </summary>
public record MapConnection(ILocation From, ILocation To, Direction Direction);

// <copyright file="SpatialContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Provides spatial awareness context for a location, calculating visibility and audibility
/// to all adjacent rooms based on door states and properties.
/// </summary>
public sealed class SpatialContext : ISpatialContext
{
    private readonly Dictionary<Direction, AdjacentRoom> _adjacentRooms;

    public ILocation CurrentLocation { get; }

    public SpatialContext(ILocation location)
    {
        CurrentLocation = location ?? throw new ArgumentNullException(nameof(location));
        _adjacentRooms = CalculateAdjacentRooms(location);
    }

    public IEnumerable<AdjacentRoom> GetAdjacentRooms() =>
        _adjacentRooms.Values.AsEnumerable();

    public IEnumerable<IItem> GetVisibleItems(int range = 1) =>
        GetAdjacentRooms()
            .Where(r => r.Visibility > 0.3f)
            .Take(range)
            .SelectMany(r => r.Location.Items)
            .Where(i => !i.HiddenFromItemList && i.GetProperty<bool>("prominent", false));

    public IEnumerable<INpc> GetAudibleNpcs(int range = 2) =>
        GetAdjacentRooms()
            .Where(r => r.Audibility > 0.3f)
            .Take(range)
            .SelectMany(r => r.Location.Npcs)
            .Where(n => n.GetProperty<bool>("visible", true));

    public bool CanSee(ILocation target)
    {
        var adjacent = _adjacentRooms.Values.FirstOrDefault(r => r.Location == target);
        return adjacent is not null && adjacent.Visibility > 0.3f;
    }

    public bool CanHear(ILocation target)
    {
        var adjacent = _adjacentRooms.Values.FirstOrDefault(r => r.Location == target);
        return adjacent is not null && adjacent.Audibility > 0.3f;
    }

    private static Dictionary<Direction, AdjacentRoom> CalculateAdjacentRooms(ILocation location)
    {
        var result = new Dictionary<Direction, AdjacentRoom>();

        foreach (var (direction, exit) in location.Exits)
        {
            var visibility = CalculateVisibility(exit);
            var audibility = CalculateAudibility(exit);
            var blockedBy = GetBlockageDescription(exit, visibility);

            result[direction] = new AdjacentRoom(
                direction,
                exit.Target,
                visibility,
                audibility,
                blockedBy
            );
        }

        return result;
    }

    private static float CalculateVisibility(Exit exit)
    {
        if (exit.Door is null)
            return 1.0f;  // Open passage = full visibility

        return exit.Door.State switch
        {
            DoorState.Open => 1.0f,
            DoorState.Closed => exit.Door.GetProperty<bool>("transparent", false) ? 0.5f : 0.0f,
            DoorState.Locked => exit.Door.GetProperty<bool>("transparent", false) ? 0.3f : 0.0f,
            _ => 0.0f
        };
    }

    private static float CalculateAudibility(Exit exit)
    {
        if (exit.Door is null)
            return 1.0f;  // Open passage = full audibility

        var soundproofing = exit.Door.GetProperty<float>("soundproofing", 0.5f);

        return exit.Door.State switch
        {
            DoorState.Open => 1.0f,
            DoorState.Closed => 1.0f - soundproofing,
            DoorState.Locked => 1.0f - soundproofing,
            _ => 0.0f
        };
    }

    private static string? GetBlockageDescription(Exit exit, float visibility)
    {
        if (exit.Door is null)
            return null;

        if (visibility > 0.0f)
            return null;  // Not fully blocked

        return exit.Door.State switch
        {
            DoorState.Closed => $"closed {exit.Door.Name.ToLowerInvariant()}",
            DoorState.Locked => $"locked {exit.Door.Name.ToLowerInvariant()}",
            _ => "unknown blockage"
        };
    }
}

// <copyright file="ISpatialContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Represents the spatial awareness context for the current location.
/// Provides access to adjacent rooms with visibility and audibility factors.
/// </summary>
public interface ISpatialContext
{
    /// <summary>
    /// Gets the current location of the player.
    /// </summary>
    ILocation CurrentLocation { get; }

    /// <summary>
    /// Gets all adjacent rooms with their visibility and audibility factors.
    /// </summary>
    IEnumerable<AdjacentRoom> GetAdjacentRooms();

    /// <summary>
    /// Gets visible items from adjacent rooms within specified range.
    /// </summary>
    IEnumerable<IItem> GetVisibleItems(int range = 1);

    /// <summary>
    /// Gets audible NPCs from adjacent rooms within specified range.
    /// </summary>
    IEnumerable<INpc> GetAudibleNpcs(int range = 2);

    /// <summary>
    /// Determines if you can see into a target location.
    /// </summary>
    bool CanSee(ILocation target);

    /// <summary>
    /// Determines if you can hear from a target location.
    /// </summary>
    bool CanHear(ILocation target);
}

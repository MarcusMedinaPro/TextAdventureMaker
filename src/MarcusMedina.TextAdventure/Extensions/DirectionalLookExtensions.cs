// <copyright file="DirectionalLookExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using System.Text;

namespace MarcusMedina.TextAdventure.Extensions;

/// <summary>
/// Extensions for looking in specific directions and peeking into adjacent rooms.
/// </summary>
public static class DirectionalLookExtensions
{
    /// <summary>
    /// Gets a brief glimpse of an adjacent room suitable for "look direction" commands.
    /// Shows room description, prominent items, and visible NPCs.
    /// </summary>
    public static string GetRoomGlimpse(this ILocation location)
    {
        ArgumentNullException.ThrowIfNull(location);

        var sb = new StringBuilder();

        // Room name/short description
        var description = location.GetDescription();
        sb.AppendLine(description);

        // Visible prominent items (up to 3)
        var visibleItems = location.Items
            .Where(i => !i.HiddenFromItemList && i.GetProperty<bool>("prominent", false))
            .Take(3)
            .Select(i => i.Name)
            .ToList();

        if (visibleItems.Count > 0)
            sb.AppendLine($"You can make out: {visibleItems.CommaJoin()}.");

        // Visible NPCs (up to 2)
        var visibleNpcs = location.Npcs
            .Where(n => n.GetProperty<bool>("visible", true))
            .Take(2)
            .Select(n => n.Name)
            .ToList();

        if (visibleNpcs.Count > 0)
            sb.AppendLine($"You see: {visibleNpcs.CommaJoin()}.");

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Determines visibility factor for an exit (0.0 = completely blocked, 1.0 = fully visible).
    /// </summary>
    public static float GetVisibility(this Exit exit)
    {
        ArgumentNullException.ThrowIfNull(exit);

        if (exit.Door is null)
            return 1.0f;  // Open passage

        return exit.Door.State switch
        {
            DoorState.Open => 1.0f,
            DoorState.Closed => exit.Door.GetProperty<bool>("transparent", false) ? 0.5f : 0.0f,
            DoorState.Locked => exit.Door.GetProperty<bool>("transparent", false) ? 0.3f : 0.0f,
            _ => 0.0f
        };
    }

    /// <summary>
    /// Checks whether you can see through an exit into the adjacent room.
    /// </summary>
    public static bool CanSeeThrough(this Exit exit) =>
        exit.GetVisibility() > 0.3f;
}

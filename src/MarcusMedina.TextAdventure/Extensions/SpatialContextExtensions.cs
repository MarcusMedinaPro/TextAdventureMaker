// <copyright file="SpatialContextExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using System.Text;

namespace MarcusMedina.TextAdventure.Extensions;

/// <summary>
/// Extensions for spatial context, providing enhanced room descriptions with awareness
/// of adjacent rooms.
/// </summary>
public static class SpatialContextExtensions
{
    /// <summary>
    /// Gets a complete spatial description including the current location and glimpses
    /// of adjacent rooms based on visibility and audibility.
    /// </summary>
    public static string GetSpatialDescription(this ISpatialContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var sb = new StringBuilder();
        sb.AppendLine(context.CurrentLocation.GetDescription());

        // Add visual glimpses of adjacent rooms
        var visuallyAccessible = context.GetAdjacentRooms()
            .Where(a => a.Visibility > 0.3f)
            .OrderBy(a => a.Direction);

        foreach (var adjacent in visuallyAccessible)
        {
            var glimpseText = adjacent.Visibility switch
            {
                >= 0.8f => $"To the {adjacent.Direction.ToString().ToLowerInvariant()}, you clearly see {adjacent.Location.Id}.",
                >= 0.5f => $"Through the {adjacent.Direction.ToString().ToLowerInvariant()}, you glimpse {adjacent.Location.Id}.",
                _ => $"To the {adjacent.Direction.ToString().ToLowerInvariant()}, you can barely make out a room."
            };
            sb.AppendLine(glimpseText);
        }

        // Add ambient sounds from adjacent rooms
        var auditoryAccessible = context.GetAdjacentRooms()
            .Where(a => a.Audibility > 0.5f)
            .OrderBy(a => a.Direction);

        foreach (var adjacent in auditoryAccessible)
        {
            var sounds = adjacent.Location.GetProperty<string>("ambient_sound", string.Empty);
            if (!string.IsNullOrEmpty(sounds))
                sb.AppendLine($"From the {adjacent.Direction.ToString().ToLowerInvariant()}, you hear {sounds}.");
        }

        return sb.ToString().Trim();
    }

    /// <summary>
    /// Creates a spatial context for the given location.
    /// </summary>
    public static ISpatialContext CreateSpatialContext(this ILocation location) =>
        new Models.SpatialContext(location);
}

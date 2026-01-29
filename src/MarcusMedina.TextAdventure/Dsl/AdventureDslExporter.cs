// <copyright file="AdventureDslExporter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

using System.Globalization;
using System.Text;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

/// <summary>
/// Exports game state to .adventure DSL format.
/// </summary>
public class AdventureDslExporter
{
    /// <summary>
    /// Exports a GameState to DSL format string.
    /// </summary>
    public string Export(GameState state, string? worldTitle = null, string? goal = null)
    {
        ArgumentNullException.ThrowIfNull(state);

        var sb = new StringBuilder();
        var exportedDoors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Header
        if (!string.IsNullOrWhiteSpace(worldTitle))
        {
            _ = sb.AppendLine($"world: {worldTitle}");
        }

        if (!string.IsNullOrWhiteSpace(goal))
        {
            _ = sb.AppendLine($"goal: {goal}");
        }

        _ = sb.AppendLine($"start: {state.CurrentLocation.Id}");
        _ = sb.AppendLine();

        // Export all locations
        foreach (var location in state.Locations)
        {
            ExportLocation(sb, location, exportedDoors);
            _ = sb.AppendLine();
        }

        return sb.ToString();
    }

    /// <summary>
    /// Exports a GameState to a .adventure file.
    /// </summary>
    public void ExportToFile(GameState state, string path, string? worldTitle = null, string? goal = null)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var content = Export(state, worldTitle, goal);
        File.WriteAllText(path, content);
    }

    /// <summary>
    /// Exports a DslAdventure back to DSL format.
    /// </summary>
    public string Export(DslAdventure adventure)
    {
        ArgumentNullException.ThrowIfNull(adventure);

        var worldTitle = adventure.Metadata.TryGetValue("world", out var w) ? w : null;
        var goal = adventure.Metadata.TryGetValue("goal", out var g) ? g : null;

        return Export(adventure.State, worldTitle, goal);
    }

    private void ExportLocation(StringBuilder sb, ILocation location, HashSet<string> exportedDoors)
    {
        // Location header
        var desc = location.GetDescription();
        _ = !string.IsNullOrWhiteSpace(desc)
            ? sb.AppendLine($"location: {location.Id} | {desc}")
            : sb.AppendLine($"location: {location.Id}");

        // Export items in this location
        foreach (var item in location.Items)
        {
            if (item is Key key)
            {
                ExportKey(sb, key);
            }
            else
            {
                ExportItem(sb, item);
            }
        }

        // Export doors (only once per door)
        foreach (var exit in location.Exits)
        {
            if (exit.Value.Door != null && !exportedDoors.Contains(exit.Value.Door.Id))
            {
                ExportDoor(sb, exit.Value.Door);
                _ = exportedDoors.Add(exit.Value.Door.Id);
            }
        }

        // Export exits
        foreach (var exit in location.Exits)
        {
            ExportExit(sb, exit.Key, exit.Value);
        }
    }

    private void ExportItem(StringBuilder sb, IItem item)
    {
        var parts = new List<string> { item.Id, item.Name };

        var description = item.GetDescription();
        if (!string.IsNullOrWhiteSpace(description))
        {
            parts.Add(description);
        }

        // Options
        var options = new List<string>();

        if (Math.Abs(item.Weight) > 0.001f)
        {
            options.Add($"weight={item.Weight.ToString(CultureInfo.InvariantCulture)}");
        }

        if (!item.Takeable)
        {
            options.Add("takeable=false");
        }

        if (item.Aliases.Count > 0)
        {
            options.Add($"aliases={string.Join(",", item.Aliases)}");
        }

        var line = string.Join(" | ", parts);
        if (options.Count > 0)
        {
            line += " | " + string.Join(" | ", options);
        }

        _ = sb.AppendLine($"item: {line}");
    }

    private void ExportKey(StringBuilder sb, Key key)
    {
        var parts = new List<string> { key.Id, key.Name };

        var description = key.GetDescription();
        if (!string.IsNullOrWhiteSpace(description))
        {
            parts.Add(description);
        }

        var options = new List<string>();

        if (Math.Abs(key.Weight) > 0.001f)
        {
            options.Add($"weight={key.Weight.ToString(CultureInfo.InvariantCulture)}");
        }

        if (key.Aliases.Count > 0)
        {
            options.Add($"aliases={string.Join(",", key.Aliases)}");
        }

        var line = string.Join(" | ", parts);
        if (options.Count > 0)
        {
            line += " | " + string.Join(" | ", options);
        }

        _ = sb.AppendLine($"key: {line}");
    }

    private void ExportDoor(StringBuilder sb, IDoor door)
    {
        var parts = new List<string> { door.Id, door.Name };

        var description = door.GetDescription();
        if (!string.IsNullOrWhiteSpace(description))
        {
            parts.Add(description);
        }

        var options = new List<string>();

        if (door.RequiredKey != null)
        {
            options.Add($"key={door.RequiredKey.Id}");
        }

        var line = string.Join(" | ", parts);
        if (options.Count > 0)
        {
            line += " | " + string.Join(" | ", options);
        }

        _ = sb.AppendLine($"door: {line}");
    }

    private void ExportExit(StringBuilder sb, Direction direction, Exit exit)
    {
        var parts = new List<string> { $"{direction.ToString().ToLowerInvariant()} -> {exit.Target.Id}" };

        if (exit.Door != null)
        {
            parts.Add($"door={exit.Door.Id}");
        }

        if (exit.IsOneWay)
        {
            parts.Add("oneway");
        }

        _ = sb.AppendLine($"exit: {string.Join(" | ", parts)}");
    }
}

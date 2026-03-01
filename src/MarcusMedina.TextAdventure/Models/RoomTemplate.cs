// <copyright file="RoomTemplate.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Template describing a room's name and description.
/// </summary>
public sealed record RoomTemplate(string Name, string Description);

/// <summary>
/// Room templates organised by theme for procedural generation.
/// Supports: dungeon, mansion, forest.
/// </summary>
public static class RoomTemplates
{
    private static readonly Dictionary<string, RoomTemplate[]> Themes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["dungeon"] =
        [
            new("Dark Corridor", "A narrow stone corridor stretches ahead, torches casting flickering shadows."),
            new("Guard Room", "An abandoned guard post with rusty weapons mounted on the walls."),
            new("Prison Cell", "A damp prison cell with iron bars and a stone bench."),
            new("Treasure Chamber", "Gold and jewels glint in the torchlight, scattered across the floor."),
            new("Armoury", "Racks of swords, shields, and armour line the walls."),
            new("Vault", "A large vault with reinforced doors and empty shelves."),
            new("Dungeons", "The lowest levels, where prisoners were kept in chains."),
            new("Grand Hall", "A massive hall with high ceilings and stone pillars."),
        ],

        ["mansion"] =
        [
            new("Foyer", "An elegant entrance hall with marble floors and a grand staircase."),
            new("Library", "Dusty books line the shelves from floor to ceiling."),
            new("Dining Room", "A long table set for a feast that never came."),
            new("Bedroom", "A four-poster bed with faded velvet curtains."),
            new("Parlour", "A cosy sitting room with comfortable furniture and a fireplace."),
            new("Study", "A scholar's study with books, papers, and a wooden desk."),
            new("Kitchen", "A large kitchen with copper pots and a stone oven."),
            new("Ballroom", "A vast empty ballroom with a polished wooden floor."),
        ],

        ["forest"] =
        [
            new("Clearing", "Sunlight filters through the tree canopy into this open space."),
            new("Dense Woods", "Tall trees press close on all sides, blocking out the sky."),
            new("Stream", "A babbling brook crosses your path, clear and cold."),
            new("Cave Entrance", "A dark opening in the hillside, trees grown around it."),
            new("Ancient Oak", "A massive oak tree, hundreds of years old, dominates the area."),
            new("Mushroom Ring", "A circle of white mushrooms grows in the grass."),
            new("Abandoned Camp", "The remnants of an old camp, with a cold firepit."),
            new("Waterfall", "A small waterfall cascades down moss-covered rocks."),
        ]
    };

    /// <summary>
    /// Gets a random template for the specified theme.
    /// </summary>
    public static RoomTemplate? GetRandomTemplate(string theme)
    {
        if (!Themes.TryGetValue(theme, out var templates) || templates.Length == 0)
            return null;

        var index = Random.Shared.Next(templates.Length);
        return templates[index];
    }

    /// <summary>
    /// Gets all templates for a theme.
    /// </summary>
    public static IEnumerable<RoomTemplate> GetTemplates(string theme) =>
        Themes.TryGetValue(theme, out var templates) ? templates : [];

    /// <summary>
    /// Gets all available theme names.
    /// </summary>
    public static IEnumerable<string> GetThemeNames() => Themes.Keys;
}

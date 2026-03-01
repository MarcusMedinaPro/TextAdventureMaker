// <copyright file="Achievements.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Predefined achievements that players can unlock during gameplay.
/// </summary>
public static class Achievements
{
    /// <summary>
    /// Unlocked when player visits 10 different locations.
    /// </summary>
    public static IAchievement Explorer => new Achievement(
        "explorer",
        "Explorer",
        "Visit 10 different locations",
        h => h.GetVisitedLocations().Count() >= 10
    );

    /// <summary>
    /// Unlocked when player completes the game without killing any NPCs.
    /// </summary>
    public static IAchievement Pacifist => new Achievement(
        "pacifist",
        "Pacifist",
        "Complete the game without killing anyone",
        h => !h.GetByType(HistoryEventType.NpcKilled).Any()
    );

    /// <summary>
    /// Unlocked when player acquires 20 different items.
    /// </summary>
    public static IAchievement Collector => new Achievement(
        "collector",
        "Collector",
        "Acquire 20 different items",
        h => h.GetAcquiredItems().Count() >= 20
    );

    /// <summary>
    /// Unlocked when player meets 15 different NPCs.
    /// </summary>
    public static IAchievement SocialButterfly => new Achievement(
        "social_butterfly",
        "Social Butterfly",
        "Meet 15 different NPCs",
        h => h.GetMetNpcs().Count() >= 15
    );

    /// <summary>
    /// Unlocked when player plays for more than 30 minutes.
    /// </summary>
    public static IAchievement Dedicated => new Achievement(
        "dedicated",
        "Dedicated",
        "Play for more than 30 minutes",
        h => h.GetTotalPlayTime().TotalMinutes >= 30
    );
}

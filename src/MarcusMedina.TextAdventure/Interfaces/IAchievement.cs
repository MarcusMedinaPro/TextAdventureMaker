// <copyright file="IAchievement.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Represents an achievement that can be unlocked based on player history conditions.
/// </summary>
public interface IAchievement
{
    /// <summary>
    /// Unique identifier for the achievement.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Display name of the achievement.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Description of what is required to unlock the achievement.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Whether the achievement has been unlocked.
    /// </summary>
    bool IsUnlocked { get; }

    /// <summary>
    /// Condition function that evaluates player history to determine if achievement should unlock.
    /// </summary>
    Func<IPlayerHistory, bool> Condition { get; }

    /// <summary>
    /// Unlocks the achievement.
    /// </summary>
    void Unlock();
}

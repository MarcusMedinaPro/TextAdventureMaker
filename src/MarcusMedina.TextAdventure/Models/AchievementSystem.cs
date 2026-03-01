// <copyright file="AchievementSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Manages achievement registration and unlock checking.
/// </summary>
public sealed class AchievementSystem
{
    private readonly List<IAchievement> _achievements = [];

    /// <summary>
    /// Registers an achievement in the system.
    /// </summary>
    public void Register(IAchievement achievement) => _achievements.Add(achievement);

    /// <summary>
    /// Checks all unlockedachievements against player history and unlocks those whose conditions are met.
    /// </summary>
    public IEnumerable<IAchievement> CheckAchievements(IPlayerHistory history)
    {
        foreach (var achievement in _achievements.Where(a => !a.IsUnlocked))
        {
            if (achievement.Condition(history))
            {
                achievement.Unlock();
                history.Record(HistoryEventType.Achievement, achievement);
                yield return achievement;
            }
        }
    }
}

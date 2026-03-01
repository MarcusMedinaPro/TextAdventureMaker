// <copyright file="Achievement.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Concrete implementation of an achievement that can be unlocked.
/// </summary>
public sealed class Achievement : IAchievement
{
    private bool _isUnlocked;

    /// <summary>
    /// Initialises a new achievement with the specified criteria.
    /// </summary>
    public Achievement(string id, string name, string description, Func<IPlayerHistory, bool> condition)
    {
        Id = id;
        Name = name;
        Description = description;
        Condition = condition;
        _isUnlocked = false;
    }

    public string Id { get; }

    public string Name { get; }

    public string Description { get; }

    public bool IsUnlocked => _isUnlocked;

    public Func<IPlayerHistory, bool> Condition { get; }

    public void Unlock() => _isUnlocked = true;
}

// <copyright file="IStats.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

public interface IStats
{
    int Health { get; }
    int MaxHealth { get; }
    void Damage(int amount);
    void Heal(int amount);
    void SetMaxHealth(int maxHealth);
    void SetHealth(int health);
}

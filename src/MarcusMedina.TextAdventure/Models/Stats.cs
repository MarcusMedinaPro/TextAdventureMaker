// <copyright file="Stats.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Stats(int maxHealth, int? currentHealth = null) : IStats
{
    public int MaxHealth { get; private set; } = ValidateMaxHealth(maxHealth);
    public int Health { get; private set; } = Clamp(currentHealth ?? maxHealth, 0, maxHealth);

    private static int ValidateMaxHealth(int maxHealth)
    {
        if (maxHealth <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxHealth));
        return maxHealth;
    }

    public void Damage(int amount)
    {
        if (amount <= 0)
            return;
        Health = Clamp(Health - amount, 0, MaxHealth);
    }

    public void Heal(int amount)
    {
        if (amount <= 0)
            return;
        Health = Clamp(Health + amount, 0, MaxHealth);
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        if (newMaxHealth <= 0)
            throw new ArgumentOutOfRangeException(nameof(newMaxHealth));
        MaxHealth = newMaxHealth;
        Health = Clamp(Health, 0, MaxHealth);
    }

    public void SetHealth(int health) => Health = Clamp(health, 0, MaxHealth);

    private static int Clamp(int value, int min, int max) =>
        value < min ? min : value > max ? max : value;
}

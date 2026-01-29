// <copyright file="Stats.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public class Stats : IStats
{
    public int Health { get; private set; }
    public int MaxHealth { get; private set; }

    public Stats(int maxHealth, int? currentHealth = null)
    {
        if (maxHealth <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxHealth));
        MaxHealth = maxHealth;
        Health = Clamp(currentHealth ?? maxHealth, 0, MaxHealth);
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

    public void SetMaxHealth(int maxHealth)
    {
        if (maxHealth <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxHealth));
        MaxHealth = maxHealth;
        Health = Clamp(Health, 0, MaxHealth);
    }

    public void SetHealth(int health) => Health = Clamp(health, 0, MaxHealth);

    private static int Clamp(int value, int min, int max) => value < min ? min : value > max ? max : value;
}

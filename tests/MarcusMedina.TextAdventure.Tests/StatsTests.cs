// <copyright file="StatsTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class StatsTests
{
    [Fact]
    public void Stats_DefaultsCurrentHealthToMax()
    {
        var stats = new Stats(10);

        Assert.Equal(10, stats.MaxHealth);
        Assert.Equal(10, stats.Health);
    }

    [Fact]
    public void Stats_HealthIsClampedOnConstruction()
    {
        var stats = new Stats(10, currentHealth: 50);

        Assert.Equal(10, stats.Health);
    }

    [Fact]
    public void Damage_CannotGoBelowZero()
    {
        var stats = new Stats(10, currentHealth: 3);

        stats.Damage(10);

        Assert.Equal(0, stats.Health);
    }

    [Fact]
    public void Heal_CannotExceedMax()
    {
        var stats = new Stats(10, currentHealth: 8);

        stats.Heal(10);

        Assert.Equal(10, stats.Health);
    }

    [Fact]
    public void SetMaxHealth_ClampsCurrentHealth()
    {
        var stats = new Stats(10, currentHealth: 10);

        stats.SetMaxHealth(6);

        Assert.Equal(6, stats.MaxHealth);
        Assert.Equal(6, stats.Health);
    }
}

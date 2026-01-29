// <copyright file="FluidTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Models;

public class FluidTests
{
    [Fact]
    public void Fluid_CanUseTupleConstructor()
    {
        Fluid water = (id: "water", name: "Water", description: "Clear and cold.");

        Assert.Equal("Water", water.Name);
        Assert.Equal("Clear and cold.", water.GetDescription());
    }
}

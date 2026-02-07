// <copyright file="PourCommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class PourCommandTests
{
    [Fact]
    public void PourCommand_PoursFluidIntoGlass()
    {
        FluidItem water = new("water", "water");
        Glass glass = new("glass", "glass");
        GameState state = new(new Location("start"));
        _ = state.Inventory.Add(water);
        _ = state.Inventory.Add(glass);

        CommandResult result = state.Execute(new PourCommand("water", "glass"));

        Assert.True(result.Success);
        Assert.DoesNotContain(state.Inventory.Items, i => i.Id == "water");
        _ = Assert.Single(glass.Contents);
    }

    [Fact]
    public void PourCommand_MissingFluid_ReturnsItemNotInInventoryError()
    {
        Glass glass = new("glass", "glass");
        GameState state = new(new Location("start"));
        _ = state.Inventory.Add(glass);

        CommandResult result = state.Execute(new PourCommand("water", "glass"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotInInventory, result.Error);
    }
}

// <copyright file="PourCommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

public class PourCommandTests
{
    [Fact]
    public void PourCommand_PoursFluidIntoGlass()
    {
        var water = new FluidItem("water", "water");
        var glass = new Glass("glass", "glass");
        var state = new GameState(new Location("start"));
        _ = state.Inventory.Add(water);
        _ = state.Inventory.Add(glass);

        var result = state.Execute(new PourCommand("water", "glass"));

        Assert.True(result.Success);
        Assert.DoesNotContain(state.Inventory.Items, i => i.Id == "water");
        _ = Assert.Single(glass.Contents);
    }

    [Fact]
    public void PourCommand_MissingFluid_ReturnsItemNotInInventoryError()
    {
        var glass = new Glass("glass", "glass");
        var state = new GameState(new Location("start"));
        _ = state.Inventory.Add(glass);

        var result = state.Execute(new PourCommand("water", "glass"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotInInventory, result.Error);
    }
}

// <copyright file="EatDrinkFallbackTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

public class EatDrinkFallbackTests
{
    [Fact]
    public void EatCommand_ItemInRoom_ReturnsMustPickUpHint()
    {
        Location room = new("room");
        IItem bread = new Item("bread", "Bread").SetFood();
        room.AddItem(bread);
        GameState state = new(room);

        CommandResult result = state.Execute(new EatCommand("bread"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
        Assert.Equal(Language.MustPickUpToEat, result.Message);
    }

    [Fact]
    public void EatCommand_ItemInInventory_Succeeds()
    {
        Location room = new("room");
        GameState state = new(room);
        IItem bread = new Item("bread", "Bread").SetFood();
        _ = state.Inventory.Add(bread);

        CommandResult result = state.Execute(new EatCommand("bread"));

        Assert.True(result.Success);
    }

    [Fact]
    public void DrinkCommand_ItemInRoom_ReturnsMustPickUpHint()
    {
        Location room = new("room");
        IItem water = new Item("water", "Water").SetDrinkable();
        room.AddItem(water);
        GameState state = new(room);

        CommandResult result = state.Execute(new DrinkCommand("water"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
        Assert.Equal(Language.MustPickUpToDrink, result.Message);
    }

    [Fact]
    public void DrinkCommand_ItemInInventory_Succeeds()
    {
        Location room = new("room");
        GameState state = new(room);
        IItem water = new Item("water", "Water").SetDrinkable();
        _ = state.Inventory.Add(water);

        CommandResult result = state.Execute(new DrinkCommand("water"));

        Assert.True(result.Success);
    }
}

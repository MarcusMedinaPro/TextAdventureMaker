// <copyright file="Slice046ConsumablesTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

public class Slice046ConsumablesTests
{
    [Fact]
    public void EatCommand_EatFood_HealsAndConsumesOne()
    {
        GameState state = new(new Location("room"), stats: new Stats(20, currentHealth: 10));
        Item bread = new("bread", "Bread");
        _ = bread.SetFood().SetHealAmount(4).SetAmount(2);
        _ = state.Inventory.Add(bread);

        CommandResult result = state.Execute(new EatCommand("bread"));

        Assert.True(result.Success);
        Assert.Equal(14, state.Stats.Health);
        Assert.Equal(1, bread.Amount);
    }

    [Fact]
    public void EatCommand_NonFood_FailsWithItemNotUsable()
    {
        GameState state = new(new Location("room"));
        Item rock = new("rock", "Rock");
        _ = state.Inventory.Add(rock);

        CommandResult result = state.Execute(new EatCommand("rock"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotUsable, result.Error);
    }

    [Fact]
    public void EatCommand_ItemNotInInventory_Fails()
    {
        GameState state = new(new Location("room"));

        CommandResult result = state.Execute(new EatCommand("bread"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
    }

    [Fact]
    public void EatCommand_EatingLastAmount_RemovesItemFromInventory()
    {
        GameState state = new(new Location("room"));
        Item berry = new("berry", "Berry");
        _ = berry.SetFood().SetAmount(1);
        _ = state.Inventory.Add(berry);

        CommandResult result = state.Execute(new EatCommand("berry"));

        Assert.True(result.Success);
        Assert.Null(state.Inventory.FindItem("berry"));
    }

    [Fact]
    public void DrinkCommand_Drinkable_Heals()
    {
        GameState state = new(new Location("room"), stats: new Stats(20, currentHealth: 7));
        Item tea = new("tea", "Tea");
        _ = tea.SetDrinkable().SetHealAmount(3);
        _ = state.Inventory.Add(tea);

        CommandResult result = state.Execute(new DrinkCommand("tea"));

        Assert.True(result.Success);
        Assert.Equal(10, state.Stats.Health);
    }

    [Fact]
    public void DrinkCommand_NonDrinkable_FailsWithItemNotUsable()
    {
        GameState state = new(new Location("room"));
        Item apple = new("apple", "Apple");
        _ = apple.SetFood();
        _ = state.Inventory.Add(apple);

        CommandResult result = state.Execute(new DrinkCommand("apple"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotUsable, result.Error);
    }

    [Fact]
    public void DrinkCommand_ItemNotInInventory_Fails()
    {
        GameState state = new(new Location("room"));

        CommandResult result = state.Execute(new DrinkCommand("water"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotFound, result.Error);
    }

    [Fact]
    public void EatCommand_PoisonedFood_AddsPoisonEffect()
    {
        GameState state = new(new Location("room"), stats: new Stats(20, currentHealth: 12));
        Item stew = new("stew", "Stew");
        _ = stew.SetFood().SetPoisoned().SetPoisonDamage(2, 3);
        _ = state.Inventory.Add(stew);

        CommandResult result = state.Execute(new EatCommand("stew"));

        Assert.True(result.Success);
        PoisonEffect poison = Assert.Single(state.ActivePoisons);
        Assert.Equal(2, poison.DamagePerTurn);
        Assert.Equal(3, poison.RemainingTurns);
    }

    [Fact]
    public void DrinkCommand_PoisonedDrink_AddsPoisonEffect()
    {
        GameState state = new(new Location("room"), stats: new Stats(20, currentHealth: 12));
        Item tonic = new("tonic", "Tonic");
        _ = tonic.SetDrinkable().SetPoisoned().SetPoisonDamage(1, 2);
        _ = state.Inventory.Add(tonic);

        CommandResult result = state.Execute(new DrinkCommand("tonic"));

        Assert.True(result.Success);
        PoisonEffect poison = Assert.Single(state.ActivePoisons);
        Assert.Equal(1, poison.DamagePerTurn);
        Assert.Equal(2, poison.RemainingTurns);
    }

    [Fact]
    public void GameState_TickPoisons_DamagesAndExpires()
    {
        GameState state = new(new Location("room"), stats: new Stats(20, currentHealth: 10));
        state.AddPoison(new PoisonEffect("wine", 2, 2));

        List<(string SourceName, int Damage)> first = state.TickPoisons();
        List<(string SourceName, int Damage)> second = state.TickPoisons();

        Assert.Single(first);
        Assert.Single(second);
        Assert.Equal(6, state.Stats.Health);
        Assert.Empty(state.ActivePoisons);
    }

    [Fact]
    public void ConsumableExtensions_AsFood_ConfiguresConsumableProperties()
    {
        IItem item = new Item("apple", "Apple").AsFood(5);

        Assert.True(item.IsFood);
        Assert.Equal(5, item.HealAmount);
    }

    [Fact]
    public void ConsumableExtensions_AsDrinkWithPoison_ConfiguresDrinkAndPoison()
    {
        IItem item = new Item("wine", "Wine").AsDrink(2).WithPoison(3, 4);

        Assert.True(item.IsDrinkable);
        Assert.True(item.IsPoisoned);
        Assert.Equal(3, item.PoisonDamagePerTurn);
        Assert.Equal(4, item.PoisonDurationTurns);
    }

    [Fact]
    public void KeywordParser_ParseDrink_ReturnsDrinkCommand()
    {
        KeywordParser parser = new(KeywordParserConfigBuilder.BritishDefaults().Build());

        ICommand command = parser.Parse("drink water");

        _ = Assert.IsType<DrinkCommand>(command);
    }
}

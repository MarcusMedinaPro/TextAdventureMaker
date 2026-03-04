// <copyright file="LookContainerTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

public class LookContainerTests
{
    [Fact]
    public void Look_ContainerWithTwoItems_ListsContents()
    {
        Location room = new("room");
        Chest chest = new("chest", "Chest");
        Item coin = new("coin", "Coin");
        Item gem = new("gem", "Gem");
        _ = chest.Add(coin);
        _ = chest.Add(gem);
        room.AddItem(chest);
        GameState state = new(room);

        CommandResult result = state.Execute(new LookCommand("chest"));

        Assert.True(result.Success);
        Assert.Contains("Coin", result.Message);
        Assert.Contains("Gem", result.Message);
    }

    [Fact]
    public void Look_EmptyContainer_ReportsEmpty()
    {
        Location room = new("room");
        Chest chest = new("chest", "Chest");
        room.AddItem(chest);
        GameState state = new(room);

        CommandResult result = state.Execute(new LookCommand("chest"));

        Assert.True(result.Success);
        Assert.Contains(Language.ContainerIsEmpty, result.Message);
    }

    [Fact]
    public void Look_NonContainerItem_NoContainerInfoAppended()
    {
        Location room = new("room");
        Item rock = new("rock", "Rock");
        _ = rock.SetDescription("A plain rock.");
        room.AddItem(rock);
        GameState state = new(room);

        CommandResult result = state.Execute(new LookCommand("rock"));

        Assert.True(result.Success);
        Assert.DoesNotContain(Language.ContainerIsEmpty, result.Message);
        Assert.Contains("A plain rock.", result.Message);
    }
}

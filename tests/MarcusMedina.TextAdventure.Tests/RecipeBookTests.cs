// <copyright file="RecipeBookTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Models;

public class RecipeBookTests
{
    [Fact]
    public void RecipeBook_CreatesItemForMatchingRecipe()
    {
        var ice = new Item("ice", "ice");
        var fire = new Item("fire", "fire");
        var book = new RecipeBook()
            .Add(new ItemCombinationRecipe("ice", "fire", () => new Item("water", "water")));

        var result = book.Combine(ice, fire);

        Assert.True(result.Success);
        _ = Assert.Single(result.Created);
        Assert.Equal("water", result.Created[0].Id);
    }

    [Fact]
    public void RecipeBook_ReturnsFailWhenNoRecipe()
    {
        var ice = new Item("ice", "ice");
        var fire = new Item("fire", "fire");
        var book = new RecipeBook();

        var result = book.Combine(ice, fire);

        Assert.False(result.Success);
        Assert.Empty(result.Created);
    }
}

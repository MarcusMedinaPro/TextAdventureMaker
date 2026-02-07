// <copyright file="RecipeBookTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class RecipeBookTests
{
    [Fact]
    public void RecipeBook_CreatesItemForMatchingRecipe()
    {
        Item ice = new("ice", "ice");
        Item fire = new("fire", "fire");
        RecipeBook book = new RecipeBook()
            .Add(new ItemCombinationRecipe("ice", "fire", () => new Item("water", "water")));

        CombinationResult result = book.Combine(ice, fire);

        Assert.True(result.Success);
        _ = Assert.Single(result.Created);
        Assert.Equal("water", result.Created[0].Id);
    }

    [Fact]
    public void RecipeBook_ReturnsFailWhenNoRecipe()
    {
        Item ice = new("ice", "ice");
        Item fire = new("fire", "fire");
        RecipeBook book = new();

        CombinationResult result = book.Combine(ice, fire);

        Assert.False(result.Success);
        Assert.Empty(result.Created);
    }
}

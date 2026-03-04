// <copyright file="RecipeBook.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public sealed class RecipeBook
{
    private readonly List<ItemCombinationRecipe> _recipes = [];

    public RecipeBook Add(ItemCombinationRecipe recipe)
    {
        _recipes.Add(recipe);
        return this;
    }

    public CombinationResult Combine(IItem a, IItem b) =>
        _recipes.FirstOrDefault(r => r.Matches(a, b)) is { } recipe
            ? CombinationResult.Ok(recipe.Create())
            : CombinationResult.Fail();
}
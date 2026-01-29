// <copyright file="RecipeBook.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public class RecipeBook
{
    private readonly List<ItemCombinationRecipe> _recipes = [];

    public RecipeBook Add(ItemCombinationRecipe recipe)
    {
        _recipes.Add(recipe);
        return this;
    }

    public CombinationResult Combine(IItem a, IItem b)
    {
        foreach (var recipe in _recipes)
        {
            if (recipe.Matches(a, b))
            {
                return CombinationResult.Ok(recipe.Create());
            }
        }

        return CombinationResult.Fail();
    }
}

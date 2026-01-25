using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class RecipeBook
{
    private readonly List<ItemCombinationRecipe> _recipes = new();

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

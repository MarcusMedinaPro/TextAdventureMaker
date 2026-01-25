using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

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
        Assert.Single(result.Created);
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

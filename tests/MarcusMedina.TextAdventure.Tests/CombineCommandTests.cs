using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class CombineCommandTests
{
    [Fact]
    public void CombineCommand_CreatesNewItemAndRemovesInputs()
    {
        var ice = new Item("ice", "ice");
        var fire = new Item("fire", "fire");
        var recipeBook = new RecipeBook()
            .Add(new ItemCombinationRecipe("ice", "fire", () => new Item("water", "water")));
        var state = new GameState(new Location("start"), inventory: new Inventory(), recipeBook: recipeBook);
        state.Inventory.Add(ice);
        state.Inventory.Add(fire);

        var result = state.Execute(new CombineCommand("ice", "fire"));

        Assert.True(result.Success);
        Assert.Equal(1, state.Inventory.Count);
        Assert.Equal("water", state.Inventory.Items[0].Id);
    }

    [Fact]
    public void CombineCommand_MissingItem_ReturnsItemNotInInventoryError()
    {
        var recipeBook = new RecipeBook()
            .Add(new ItemCombinationRecipe("ice", "fire", () => new Item("water", "water")));
        var state = new GameState(new Location("start"), inventory: new Inventory(), recipeBook: recipeBook);
        state.Inventory.Add(new Item("ice", "ice"));

        var result = state.Execute(new CombineCommand("ice", "fire"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotInInventory, result.Error);
    }
}

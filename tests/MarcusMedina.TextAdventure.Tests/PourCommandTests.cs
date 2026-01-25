using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class PourCommandTests
{
    [Fact]
    public void PourCommand_PoursFluidIntoGlass()
    {
        var water = new FluidItem("water", "water");
        var glass = new Glass("glass", "glass");
        var state = new GameState(new Location("start"));
        state.Inventory.Add(water);
        state.Inventory.Add(glass);

        var result = state.Execute(new PourCommand("water", "glass"));

        Assert.True(result.Success);
        Assert.DoesNotContain(state.Inventory.Items, i => i.Id == "water");
        Assert.Single(glass.Contents);
    }

    [Fact]
    public void PourCommand_MissingFluid_ReturnsItemNotInInventoryError()
    {
        var glass = new Glass("glass", "glass");
        var state = new GameState(new Location("start"));
        state.Inventory.Add(glass);

        var result = state.Execute(new PourCommand("water", "glass"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotInInventory, result.Error);
    }
}

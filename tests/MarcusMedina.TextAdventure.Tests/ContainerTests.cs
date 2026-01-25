using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class ContainerTests
{
    [Fact]
    public void Glass_HoldsFluids()
    {
        var glass = new Glass("glass", "glass");
        IFluid water = new Fluid("water", "water");

        Assert.True(glass.Add(water));
        Assert.Single(glass.Contents);
    }

    [Fact]
    public void Chest_HoldsItems()
    {
        var chest = new Chest("chest", "chest");
        var coin = new Item("coin", "coin");

        Assert.True(chest.Add(coin));
        Assert.Single(chest.Contents);
    }

    [Fact]
    public void Container_RespectsMaxCount()
    {
        var chest = new Chest("chest", "chest", maxCount: 1);
        Assert.True(chest.Add(new Item("coin", "coin")));
        Assert.False(chest.Add(new Item("gem", "gem")));
    }
}

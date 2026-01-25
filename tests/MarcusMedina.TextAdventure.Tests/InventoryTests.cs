using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class InventoryTests
{
    [Fact]
    public void Inventory_TotalWeight_SumsItems()
    {
        var inventory = new Inventory();
        inventory.Add(new Item("rock", "Rock").SetWeight(1.5f));
        inventory.Add(new Item("coin", "Coin").SetWeight(0.5f));

        Assert.Equal(2.0f, inventory.TotalWeight);
    }

    [Fact]
    public void Inventory_ByCount_RespectsLimit()
    {
        var inventory = new Inventory(InventoryLimitType.ByCount, maxCount: 1);
        var item1 = new Item("rock", "Rock");
        var item2 = new Item("coin", "Coin");

        Assert.True(inventory.Add(item1));
        Assert.False(inventory.Add(item2));
    }

    [Fact]
    public void Inventory_ByWeight_RespectsLimit()
    {
        var inventory = new Inventory(InventoryLimitType.ByWeight, maxWeight: 2f);
        var item1 = new Item("rock", "Rock").SetWeight(1.5f);
        var item2 = new Item("coin", "Coin").SetWeight(1f);

        Assert.True(inventory.Add(item1));
        Assert.False(inventory.Add(item2));
    }

    [Fact]
    public void Inventory_FindItem_UsesAliases()
    {
        var inventory = new Inventory();
        var item = new Item("sword", "Sword").AddAliases("blade");
        inventory.Add(item);

        Assert.Equal(item, inventory.FindItem("blade"));
    }
}

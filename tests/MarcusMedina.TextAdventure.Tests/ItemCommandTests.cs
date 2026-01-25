using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class ItemCommandTests
{
    [Fact]
    public void TakeCommand_MovesItemToInventory()
    {
        var location = new Location("room");
        var item = new Item("apple", "Apple").SetWeight(1f);
        location.AddItem(item);
        var state = new GameState(location);

        var result = state.Execute(new TakeCommand("apple"));

        Assert.True(result.Success);
        Assert.Empty(location.Items);
        Assert.Single(state.Inventory.Items);
    }

    [Fact]
    public void TakeCommand_FailsWhenNotTakeable()
    {
        var location = new Location("room");
        var item = new Item("statue", "Statue").SetTakeable(false);
        location.AddItem(item);
        var state = new GameState(location);

        var result = state.Execute(new TakeCommand("statue"));

        Assert.False(result.Success);
        Assert.Equal(Language.CannotTakeItem, result.Message);
    }

    [Fact]
    public void DropCommand_MovesItemToLocation()
    {
        var location = new Location("room");
        var state = new GameState(location);
        var item = new Item("coin", "Coin");
        state.Inventory.Add(item);

        var result = state.Execute(new DropCommand("coin"));

        Assert.True(result.Success);
        Assert.Single(location.Items);
        Assert.Empty(state.Inventory.Items);
    }

    [Fact]
    public void InventoryCommand_ShowsTotalWeight()
    {
        var location = new Location("room");
        var state = new GameState(location);
        state.Inventory.Add(new Item("rock", "Rock").SetWeight(2f));

        var result = state.InventoryView();

        Assert.Contains(Language.TotalWeight(2f), result.Message);
    }

    [Fact]
    public void UseCommand_RequiresItemInInventory()
    {
        var location = new Location("room");
        var state = new GameState(location);

        var result = state.Execute(new UseCommand("wand"));

        Assert.False(result.Success);
        Assert.Equal(Language.NoSuchItemInventory, result.Message);
    }

    [Fact]
    public void TakeAllCommand_TakesAllTakeableItems()
    {
        var location = new Location("room");
        location.AddItem(new Item("coin", "Coin"));
        location.AddItem(new Item("apple", "Apple"));
        var state = new GameState(location);

        var result = state.Execute(new TakeAllCommand());

        Assert.True(result.Success);
        Assert.Empty(location.Items);
        Assert.Equal(2, state.Inventory.Count);
    }

    [Fact]
    public void TakeAllCommand_SkipsTooHeavyItems()
    {
        var location = new Location("room");
        location.AddItem(new Item("rock", "Rock").SetWeight(5f));
        var inventory = new Inventory(InventoryLimitType.ByWeight, maxWeight: 1f);
        var state = new GameState(location, inventory: inventory);

        var result = state.Execute(new TakeAllCommand());

        Assert.False(result.Success);
        Assert.Equal(Language.TooHeavy, result.Message);
    }

    [Fact]
    public void DropAllCommand_DropsEverything()
    {
        var location = new Location("room");
        var state = new GameState(location);
        state.Inventory.Add(new Item("coin", "Coin"));
        state.Inventory.Add(new Item("apple", "Apple"));

        var result = state.Execute(new DropAllCommand());

        Assert.True(result.Success);
        Assert.Equal(0, state.Inventory.Count);
        Assert.Equal(2, location.Items.Count);
    }
}

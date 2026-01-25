using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Helpers;

namespace MarcusMedina.TextAdventure.Tests;

public class ItemTests
{
    [Fact]
    public void Item_Matches_Name_And_Alias()
    {
        var item = new Item("sword", "Sword").AddAliases("blade", "steel");

        Assert.True(item.Matches("sword"));
        Assert.True(item.Matches("Blade"));
        Assert.False(item.Matches("hammer"));
    }

    [Fact]
    public void Item_InvalidIdOrName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Item("", "Item"));
        Assert.Throws<ArgumentException>(() => new Item("id", ""));
        Assert.Throws<ArgumentNullException>(() => new Item(null!, "Item"));
        Assert.Throws<ArgumentNullException>(() => new Item("id", null!));
    }

    [Fact]
    public void Item_CanHaveDescription()
    {
        var item = new Item("coin", "Coin")
            .Description("A shiny gold coin.");

        Assert.Equal("A shiny gold coin.", item.GetDescription());
    }

    [Fact]
    public void Key_CanHaveDescription()
    {
        var key = new Key("key1", "Brass Key")
            .Description("A small brass key with a lion crest.");

        Assert.Equal("A small brass key with a lion crest.", key.GetDescription());
    }

    [Fact]
    public void ItemFactory_DerivesIdFromName()
    {
        var item = ItemFactory.NewItem("Red Apple", 0.5f);

        Assert.Equal("red_apple", item.Id);
        Assert.Equal(0.5f, item.Weight);
    }
}

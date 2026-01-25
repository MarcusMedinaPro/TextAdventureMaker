using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Extensions;

namespace MarcusMedina.TextAdventure.Tests;

public class GameListTests
{
    [Fact]
    public void AddMany_CreatesItemsWithFactory()
    {
        var list = new GameList<Item>(name => new Item(name.ToId(), name));
        list.AddMany("cat", "rubber chicken");

        Assert.Equal("cat", list["cat"].Name);
        Assert.Equal("rubber_chicken", list["rubber chicken"].Id);
    }

    [Fact]
    public void Find_UsesItemAliasesWhenAvailable()
    {
        var list = new GameList<Item>(name => new Item(name.ToId(), name));
        list.Add("cat").AddAliases("kitten");

        var found = list.Find("kitten");

        Assert.NotNull(found);
        Assert.Equal("cat", found!.Name);
    }

    [Fact]
    public void Find_MatchesByIdOrNameForDoors()
    {
        var list = new GameList<Door>(name => new Door(name.ToId(), name));
        list.AddMany("shed door");

        Assert.NotNull(list.Find("shed_door"));
        Assert.NotNull(list.Find("shed door"));
    }
}

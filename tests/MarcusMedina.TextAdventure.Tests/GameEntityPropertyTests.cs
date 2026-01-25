using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class GameEntityPropertyTests
{
    [Fact]
    public void SetProperty_StoresValueOnItem()
    {
        var item = new Item("cup", "cup");

        item.SetProperty("hint", "A plain cup.");

        Assert.Equal("A plain cup.", item.GetHint());
    }

    [Fact]
    public void SetProperty_StoresValueOnDoor()
    {
        var door = new Door("door", "door");

        door.SetHint("It needs a key.");

        Assert.Equal("It needs a key.", door.GetHint());
    }

    [Fact]
    public void SetProperty_StoresValueOnNpc()
    {
        var npc = new Npc("cat", "cat");

        npc.SetProperty("mood", "curious");

        Assert.Equal("curious", npc.GetProperty("mood"));
    }
}

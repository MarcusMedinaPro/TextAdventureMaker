using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class NpcTests
{
    [Fact]
    public void Npc_TracksStateAndDescription()
    {
        var npc = new Npc("fox", "Fox")
            .Description("A friendly forest fox.")
            .SetState(NpcState.Friendly);

        Assert.Equal(NpcState.Friendly, npc.State);
        Assert.True(npc.IsAlive);
        Assert.Equal("A friendly forest fox.", npc.GetDescription());
    }

    [Fact]
    public void Location_CanFindNpcByName()
    {
        var location = new Location("clearing");
        var npc = new Npc("fox", "Fox");
        location.AddNpc(npc);

        Assert.Equal(npc, location.FindNpc("fox"));
    }
}

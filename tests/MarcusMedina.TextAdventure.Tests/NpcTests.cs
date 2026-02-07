// <copyright file="NpcTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class NpcTests
{
    [Fact]
    public void Npc_TracksStateAndDescription()
    {
        INpc npc = new Npc("fox", "Fox")
            .Description("A friendly forest fox.")
            .SetState(NpcState.Friendly);

        Assert.Equal(NpcState.Friendly, npc.State);
        Assert.True(npc.IsAlive);
        Assert.Equal("A friendly forest fox.", npc.GetDescription());
    }

    [Fact]
    public void Location_CanFindNpcByName()
    {
        Location location = new("clearing");
        Npc npc = new("fox", "Fox");
        location.AddNpc(npc);

        Assert.Equal(npc, location.FindNpc("fox"));
    }
}

// <copyright file="RuleBasedDialogTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class RuleBasedDialogTests
{
    [Fact]
    public void RuleBasedDialog_PicksMostSpecificRule()
    {
        var npc = new Npc("fox", "Fox");
        npc.AddDialogRule("fallback")
            .When(_ => true)
            .Say("Hello.");

        npc.AddDialogRule("specific")
            .When(_ => true)
            .When(_ => true)
            .Say("Specific hello.");

        var state = new GameState(new Location("start"));

        var result = npc.GetRuleBasedDialog(state);

        Assert.Equal("Specific hello.", result);
    }

    [Fact]
    public void RuleBasedDialog_UsesPriorityForTies()
    {
        var npc = new Npc("fox", "Fox");
        npc.AddDialogRule("low")
            .When(_ => true)
            .Priority(0)
            .Say("Low.");

        npc.AddDialogRule("high")
            .When(_ => true)
            .Priority(10)
            .Say("High.");

        var state = new GameState(new Location("start"));

        var result = npc.GetRuleBasedDialog(state);

        Assert.Equal("High.", result);
    }

    [Fact]
    public void RuleBasedDialog_ThenActionUpdatesMemory()
    {
        var npc = new Npc("fox", "Fox");
        npc.AddDialogRule("remember")
            .Say("Remembered.")
            .Then(ctx => ctx.NpcMemory.MarkSaid("remembered"));

        var state = new GameState(new Location("start"));

        var result = npc.GetRuleBasedDialog(state);

        Assert.Equal("Remembered.", result);
        Assert.True(npc.Memory.HasSaid("remembered"));
    }

    [Fact]
    public void RuleBasedDialog_FirstMeetingOnlyOnce()
    {
        var npc = new Npc("fox", "Fox");
        npc.AddDialogRule("first")
            .When(ctx => ctx.FirstMeeting)
            .Say("First time.");

        npc.AddDialogRule("repeat")
            .Say("Again.");

        var state = new GameState(new Location("start"));

        var first = npc.GetRuleBasedDialog(state);
        var second = npc.GetRuleBasedDialog(state);

        Assert.Equal("First time.", first);
        Assert.Equal("Again.", second);
    }
}

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
        Npc npc = new("fox", "Fox");
        _ = npc.AddDialogRule("fallback")
            .When(_ => true)
            .Say("Hello.");

        _ = npc.AddDialogRule("specific")
            .When(_ => true)
            .When(_ => true)
            .Say("Specific hello.");

        GameState state = new(new Location("start"));

        string? result = npc.GetRuleBasedDialog(state);

        Assert.Equal("Specific hello.", result);
    }

    [Fact]
    public void RuleBasedDialog_UsesPriorityForTies()
    {
        Npc npc = new("fox", "Fox");
        _ = npc.AddDialogRule("low")
            .When(_ => true)
            .Priority(0)
            .Say("Low.");

        _ = npc.AddDialogRule("high")
            .When(_ => true)
            .Priority(10)
            .Say("High.");

        GameState state = new(new Location("start"));

        string? result = npc.GetRuleBasedDialog(state);

        Assert.Equal("High.", result);
    }

    [Fact]
    public void RuleBasedDialog_ThenActionUpdatesMemory()
    {
        Npc npc = new("fox", "Fox");
        _ = npc.AddDialogRule("remember")
            .Say("Remembered.")
            .Then(ctx => ctx.NpcMemory.MarkSaid("remembered"));

        GameState state = new(new Location("start"));

        string? result = npc.GetRuleBasedDialog(state);

        Assert.Equal("Remembered.", result);
        Assert.True(npc.Memory.HasSaid("remembered"));
    }

    [Fact]
    public void RuleBasedDialog_FirstMeetingOnlyOnce()
    {
        Npc npc = new("fox", "Fox");
        _ = npc.AddDialogRule("first")
            .When(ctx => ctx.FirstMeeting)
            .Say("First time.");

        _ = npc.AddDialogRule("repeat")
            .Say("Again.");

        GameState state = new(new Location("start"));

        string? first = npc.GetRuleBasedDialog(state);
        string? second = npc.GetRuleBasedDialog(state);

        Assert.Equal("First time.", first);
        Assert.Equal("Again.", second);
    }
}

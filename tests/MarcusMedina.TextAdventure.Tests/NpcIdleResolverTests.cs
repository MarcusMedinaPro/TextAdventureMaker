// <copyright file="NpcIdleResolverTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class NpcIdleResolverTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static GameState StateWithNpc(out Npc npc, int interval = 3, params string[] messages)
    {
        var room = new Location("hall");
        npc = new Npc("keeper", "Keeper");
        npc.AddIdleBehavior(interval, messages.Length > 0 ? messages : ["picking his nose", "humming a song"]);
        room.AddNpc(npc);
        return new GameState(room);
    }

    private static CommandResult Look(GameState state) =>
        state.Execute(new LookCommand());

    // ── NpcIdleBehavior unit tests ────────────────────────────────────────────

    [Fact]
    public void NpcIdleBehavior_ReturnsNullBeforeInterval()
    {
        var b = new NpcIdleBehavior(3, ["hello"]);
        Assert.Null(b.Tick()); // step 1
        Assert.Null(b.Tick()); // step 2
    }

    [Fact]
    public void NpcIdleBehavior_ReturnsMessageAtInterval()
    {
        var b = new NpcIdleBehavior(3, ["hello"]);
        b.Tick(); b.Tick();
        Assert.Equal("hello", b.Tick()); // step 3
    }

    [Fact]
    public void NpcIdleBehavior_FiresAgainAtDoubleInterval()
    {
        var b = new NpcIdleBehavior(3, ["hello"]);
        for (int i = 0; i < 5; i++) b.Tick();
        Assert.Equal("hello", b.Tick()); // step 6
    }

    [Fact]
    public void NpcIdleBehavior_Interval1_FiresEveryTick()
    {
        var b = new NpcIdleBehavior(1, ["ping"]);
        Assert.Equal("ping", b.Tick());
        Assert.Equal("ping", b.Tick());
    }

    [Fact]
    public void NpcIdleBehavior_ThrowsOnInvalidInterval()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NpcIdleBehavior(0, ["msg"]));
    }

    [Fact]
    public void NpcIdleBehavior_ThrowsOnEmptyMessages()
    {
        Assert.Throws<ArgumentException>(() => new NpcIdleBehavior(3, []));
    }

    // ── Npc.AddIdleBehavior ───────────────────────────────────────────────────

    [Fact]
    public void Npc_AddIdleBehavior_AddsToList()
    {
        var npc = new Npc("foo", "Foo");
        npc.AddIdleBehavior(2, "wave", "scratch head");
        Assert.Single(npc.IdleBehaviors);
        Assert.Equal(2, npc.IdleBehaviors[0].Interval);
    }

    // ── Integration: NpcIdleResolver via Execute ──────────────────────────────

    [Fact]
    public void IdleResolver_NoMessageBeforeInterval()
    {
        var state = StateWithNpc(out _, interval: 3);
        var result = Look(state);
        Assert.Empty(result.ReactionsList); // turn 1, interval 3 — no idle yet
    }

    [Fact]
    public void IdleResolver_ShowsMessageAtInterval()
    {
        var state = StateWithNpc(out _, interval: 3, "picking his nose");
        Look(state); // turn 1
        Look(state); // turn 2
        var result = Look(state); // turn 3
        Assert.Single(result.ReactionsList);
        Assert.Equal("Keeper: picking his nose", result.ReactionsList[0]);
    }

    [Fact]
    public void IdleResolver_MessagePrefixedWithNpcName()
    {
        var state = StateWithNpc(out _, interval: 1, "yawning");
        var result = Look(state); // turn 1
        Assert.Contains("Keeper:", result.ReactionsList[0]);
    }

    [Fact]
    public void IdleResolver_SkipsIdleWhenReactionFires()
    {
        var room = new Location("hall");
        var npc = new Npc("guard", "Guard");
        npc.AddReaction("attack", "The guard raises his sword!");
        npc.AddIdleBehavior(1, "tapping foot"); // fires every command
        room.AddNpc(npc);

        var state = new GameState(room);
        var result = state.Execute(new AttackCommand(null));
        // Should have reaction but NOT idle
        Assert.Contains(result.ReactionsList, r => r.Contains("raises his sword"));
        Assert.DoesNotContain(result.ReactionsList, r => r.Contains("tapping foot"));
    }

    [Fact]
    public void IdleResolver_EachNpcTracksCounterIndependently()
    {
        var room = new Location("hall");
        var npc1 = new Npc("a", "Alice");
        var npc2 = new Npc("b", "Bob");
        npc1.AddIdleBehavior(2, "smiling");
        npc2.AddIdleBehavior(3, "frowning");
        room.AddNpc(npc1);
        room.AddNpc(npc2);

        var state = new GameState(room);
        var result1 = Look(state); // turn 1: neither fires
        var result2 = Look(state); // turn 2: Alice fires (2), Bob doesn't (3)
        var result3 = Look(state); // turn 3: Alice doesn't (4), Bob fires (3)

        Assert.Empty(result1.ReactionsList);
        Assert.Single(result2.ReactionsList);
        Assert.Contains("Alice:", result2.ReactionsList[0]);
        Assert.Single(result3.ReactionsList);
        Assert.Contains("Bob:", result3.ReactionsList[0]);
    }

    [Fact]
    public void IdleResolver_DeadNpcDoesNotIdle()
    {
        var state = StateWithNpc(out Npc npc, interval: 1, "groaning");
        npc.SetState(Enums.NpcState.Dead);
        var result = Look(state);
        Assert.Empty(result.ReactionsList);
    }

    // ── DSL parsing ───────────────────────────────────────────────────────────

    [Fact]
    public void DslParser_ParsesNpcIdle()
    {
        const string dsl = """
            world: Test | A test world.
            location: hall | The Hall | A plain hall.
            npc: keeper | name=Keeper | state=friendly | description=The keeper.
            npc_place: hall | keeper
            npc_idle: keeper | 3 | picking his nose | scratching his head | humming a song
            """;

        DslParser parser = new();
        _ = parser.ParseString(dsl);

        var idle = parser.GetNpcIdleBehaviors();
        Assert.Single(idle);
        Assert.Equal("keeper", idle[0].NpcId);
        Assert.Equal(3, idle[0].Interval);
        Assert.Equal(3, idle[0].Messages.Count);
        Assert.Contains("picking his nose", idle[0].Messages);
    }

    [Fact]
    public void DslParser_NpcIdle_AppliedAtRuntime()
    {
        const string dsl = """
            world: Test | A test world.
            location: hall | The Hall | A plain hall.
            npc: keeper | name=Keeper | state=friendly | description=The keeper.
            npc_place: hall | keeper
            npc_idle: keeper | 3 | picking his nose
            """;

        DslParser parser = new();
        DslAdventure adventure = parser.ParseString(dsl);

        INpc? npc = adventure.State.Locations.First(l => l.Id == "hall").FindNpc("keeper");
        Assert.NotNull(npc);
        Assert.Single(npc!.IdleBehaviors);
        Assert.Equal(3, npc.IdleBehaviors[0].Interval);
    }
}

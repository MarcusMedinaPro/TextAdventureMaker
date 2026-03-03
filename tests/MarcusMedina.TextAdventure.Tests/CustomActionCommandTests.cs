// <copyright file="CustomActionCommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;
using MarcusMedina.TextAdventure.Parsing;

namespace MarcusMedina.TextAdventure.Tests;

public class CustomActionCommandTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static GameState StateWithGuard(out Location room, out INpc guard)
    {
        room = new Location("gatehouse");
        var npc = new Npc("guard", "Guard");
        npc.AddReaction("blow:trumpet", "The guard covers his ears. \"Must you do that here?!\"");
        npc.AddReaction("blow",         "The guard gives you a very long, very disapproving stare.");
        npc.AddReaction("threaten",     "The guard raises an eyebrow slowly.");
        npc.AddReaction("attack",       "The guard's hand moves to his sword hilt.");
        npc.AddReaction("take",         "The guard watches your hands carefully.");
        room.AddNpc(npc);
        guard = npc;
        return new GameState(room);
    }

    private static KeywordParser ParserWithVerbs(params string[] verbs)
    {
        var builder = KeywordParserConfigBuilder.BritishDefaults();
        builder.AddCustomVerbs(verbs);
        return new KeywordParser(builder.Build());
    }

    // ── Task 094.1 — CustomActionCommand ─────────────────────────────────────

    [Fact]
    public void CustomVerb_ParsedAsCustomActionCommand()
    {
        KeywordParser parser = ParserWithVerbs("blow");
        ICommand cmd = parser.Parse("blow trumpet");

        var custom = Assert.IsType<CustomActionCommand>(cmd);
        Assert.Equal("blow", custom.Verb);
        Assert.Equal("trumpet", custom.Target);
    }

    [Fact]
    public void CustomVerb_NoTarget_ParsedCorrectly()
    {
        KeywordParser parser = ParserWithVerbs("juggle");
        ICommand cmd = parser.Parse("juggle");

        var custom = Assert.IsType<CustomActionCommand>(cmd);
        Assert.Equal("juggle", custom.Verb);
        Assert.Null(custom.Target);
    }

    // ── Task 094.2 — NpcReaction model ───────────────────────────────────────

    [Fact]
    public void NpcReaction_Fires_OnCustomVerb()
    {
        GameState state = StateWithGuard(out _, out _);
        CommandResult result = state.Execute(new CustomActionCommand("blow", "trumpet"));

        Assert.Contains(result.ReactionsList, r => r.Contains("covers his ears"));
    }

    [Fact]
    public void NpcReaction_SpecificTarget_TakesPrecedence()
    {
        GameState state = StateWithGuard(out _, out _);
        CommandResult result = state.Execute(new CustomActionCommand("blow", "trumpet"));

        // Specific "blow:trumpet" fires, not general "blow"
        Assert.DoesNotContain(result.ReactionsList, r => r.Contains("disapproving stare"));
        Assert.Contains(result.ReactionsList, r => r.Contains("covers his ears"));
    }

    [Fact]
    public void NpcReaction_FallsBackToGeneral_WhenNoSpecific()
    {
        GameState state = StateWithGuard(out _, out _);
        CommandResult result = state.Execute(new CustomActionCommand("blow", "bottle"));

        // No "blow:bottle" → falls back to "blow"
        Assert.Contains(result.ReactionsList, r => r.Contains("disapproving stare"));
    }

    [Fact]
    public void NpcReaction_Respects_Condition()
    {
        Location room = new("gatehouse");
        var npc = new Npc("guard", "Guard");
        npc.AddReaction("blow", "He covers his ears.", state => state.WorldState.GetFlag("guard_on_duty"));
        room.AddNpc(npc);
        GameState gameState = new(room);

        // Flag not set → no reaction
        CommandResult without = gameState.Execute(new CustomActionCommand("blow", null));
        Assert.Empty(without.ReactionsList);

        // Set flag → reaction fires
        gameState.WorldState.SetFlag("guard_on_duty", true);
        CommandResult with = gameState.Execute(new CustomActionCommand("blow", null));
        Assert.Contains(with.ReactionsList, r => r.Contains("covers his ears"));
    }

    [Fact]
    public void NpcReaction_Silent_WhenNpcNotInRoom()
    {
        Location playerRoom = new("gatehouse");
        Location otherRoom = new("courtyard");
        var npc = new Npc("guard", "Guard");
        npc.AddReaction("blow", "The guard gives you a very long, very disapproving stare.");
        otherRoom.AddNpc(npc); // guard is elsewhere
        GameState state = new(playerRoom);

        CommandResult result = state.Execute(new CustomActionCommand("blow", null));

        Assert.Empty(result.ReactionsList);
    }

    // ── Task 094.3 — C# API: AddCustomVerb(s) ────────────────────────────────

    [Fact]
    public void AddCustomVerbs_RegistersMultipleVerbs()
    {
        var parser = ParserWithVerbs("blow", "threaten", "juggle");

        Assert.IsType<CustomActionCommand>(parser.Parse("blow"));
        Assert.IsType<CustomActionCommand>(parser.Parse("threaten guard"));
        Assert.IsType<CustomActionCommand>(parser.Parse("juggle"));
    }

    // ── Task 094.4 — DSL parser ───────────────────────────────────────────────

    [Fact]
    public void DslParser_Registers_CustomCommands()
    {
        const string dsl = """
            world: Guard Test | A quick test.
            room: gatehouse | The Gatehouse | A stone gatehouse.
            command: blow, threaten
            """;

        DslParser parser = new();
        _ = parser.ParseString(dsl);

        var verbs = parser.GetParserConfiguration().CustomVerbs.Select(v => v.Verb).ToList();
        Assert.Contains("blow", verbs);
        Assert.Contains("threaten", verbs);
    }

    [Fact]
    public void DslParser_Registers_NpcReaction()
    {
        const string dsl = """
            world: Guard Test | A quick test.
            room: gatehouse | The Gatehouse | A stone gatehouse.
            npc: guard | name=Guard | state=friendly | description=A palace guard.
            npc_place: gatehouse | guard
            npc_reaction: guard | on=blow | text=The guard gives you a very long, very disapproving stare.
            """;

        DslParser dslParser = new();
        DslAdventure adventure = dslParser.ParseString(dsl);

        ILocation? room = adventure.State.Locations.FirstOrDefault(l => l.Id == "gatehouse");
        Assert.NotNull(room);

        INpc? guard = room!.FindNpc("guard");
        Assert.NotNull(guard);

        string? reaction = guard!.GetReaction("blow", adventure.State);
        Assert.NotNull(reaction);
        Assert.Contains("disapproving stare", reaction);
    }
}

// <copyright file="TalkCommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class TalkCommandTests
{
    [Fact]
    public void TalkCommand_RequiresTarget()
    {
        var location = new Location("camp");
        var state = new GameState(location);
        var command = new TalkCommand(null);

        var result = command.Execute(new CommandContext(state));

        Assert.False(result.Success);
        Assert.Equal(GameError.MissingArgument, result.Error);
        Assert.Equal(Language.NoOneToTalkTo, result.Message);
    }

    [Fact]
    public void TalkCommand_FailsWhenNpcMissing()
    {
        var location = new Location("camp");
        var state = new GameState(location);
        var command = new TalkCommand("fox");

        var result = command.Execute(new CommandContext(state));

        Assert.False(result.Success);
        Assert.Equal(GameError.TargetNotFound, result.Error);
        Assert.Equal(Language.NoSuchNpcHere, result.Message);
    }

    [Fact]
    public void TalkCommand_OutputsDialogAndOptions()
    {
        var location = new Location("camp");
        var npc = new Npc("fox", "Fox")
            .SetDialog(new DialogNode("Hello traveler.")
                .AddOption("Ask about the forest")
                .AddOption("Say goodbye"));
        location.AddNpc(npc);
        var state = new GameState(location);
        var command = new TalkCommand("fox");

        var result = command.Execute(new CommandContext(state));

        Assert.True(result.Success);
        Assert.Contains("Hello traveler.", result.Message);
        Assert.Contains(Language.DialogOptionsLabel, result.Message);
        Assert.Contains("1. Ask about the forest", result.Message);
        Assert.Contains("2. Say goodbye", result.Message);
    }

    [Fact]
    public void TalkCommand_HandlesNpcWithoutDialog()
    {
        var location = new Location("camp");
        location.AddNpc(new Npc("fox", "Fox"));
        var state = new GameState(location);
        var command = new TalkCommand("fox");

        var result = command.Execute(new CommandContext(state));

        Assert.True(result.Success);
        Assert.Equal(Language.NpcHasNothingToSay, result.Message);
    }
}

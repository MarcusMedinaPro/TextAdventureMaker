// <copyright file="WrongObjectTypeTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

public class WrongObjectTypeTests
{
    [Fact]
    public void TalkCommand_TargetIsItem_ReturnsWrongObjectType()
    {
        Location room = new("room");
        room.AddItem(new Item("door", "Door"));
        GameState state = new(room);

        CommandResult result = state.Execute(new TalkCommand("door"));

        Assert.False(result.Success);
        Assert.Equal(GameError.WrongObjectType, result.Error);
        Assert.Equal(Language.CannotTalkToThat, result.Message);
    }

    [Fact]
    public void TalkCommand_TargetNotFound_ReturnsTargetNotFound()
    {
        Location room = new("room");
        GameState state = new(room);

        CommandResult result = state.Execute(new TalkCommand("ghost"));

        Assert.False(result.Success);
        Assert.Equal(GameError.TargetNotFound, result.Error);
    }

    [Fact]
    public void AttackCommand_TargetIsItem_ReturnsWrongObjectType()
    {
        Location room = new("room");
        room.AddItem(new Item("barrel", "Barrel"));
        GameState state = new(room);

        CommandResult result = state.Execute(new AttackCommand("barrel"));

        Assert.False(result.Success);
        Assert.Equal(GameError.WrongObjectType, result.Error);
        Assert.Equal(Language.CannotAttackThat, result.Message);
    }

    [Fact]
    public void AttackCommand_TargetNotFound_ReturnsTargetNotFound()
    {
        Location room = new("room");
        GameState state = new(room);

        CommandResult result = state.Execute(new AttackCommand("nobody"));

        Assert.False(result.Success);
        Assert.Equal(GameError.TargetNotFound, result.Error);
    }
}

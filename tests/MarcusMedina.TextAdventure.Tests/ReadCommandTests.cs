// <copyright file="ReadCommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class ReadCommandTests
{
    [Fact]
    public void ReadCommand_ReadsReadableItemInLocation()
    {
        IItem sign = new Item("sign", "Sign")
            .SetTakeable(false)
            .SetReadable()
            .SetReadText("Welcome.");
        Location location = new("start");
        location.AddItem(sign);
        GameState state = new(location);

        CommandResult result = state.Execute(new ReadCommand("sign"));

        Assert.True(result.Success);
        Assert.Equal("Welcome.", result.Message);
    }

    [Fact]
    public void ReadCommand_RequiresTakeWhenConfigured()
    {
        IItem paper = new Item("paper", "Paper")
            .SetReadable()
            .RequireTakeToRead()
            .SetReadText("News!");
        Location location = new("start");
        location.AddItem(paper);
        GameState state = new(location);

        CommandResult result = state.Execute(new ReadCommand("paper"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotInInventory, result.Error);
    }

    [Fact]
    public void ReadCommand_ReadingCostShowsMessage()
    {
        IItem tome = new Item("tome", "Tome")
            .SetReadable()
            .SetReadingCost(3)
            .SetReadText("Secrets...");
        Location location = new("start");
        location.AddItem(tome);
        GameState state = new(location);

        CommandResult result = state.Execute(new ReadCommand("tome"));

        Assert.True(result.Success);
        Assert.Contains("3", result.Message);
        Assert.Contains("Secrets", result.Message);
    }

    [Fact]
    public void ReadCommand_RequiresConditionToRead()
    {
        IItem letter = new Item("letter", "Letter")
            .SetReadable()
            .RequiresToRead(s => s.Inventory.Items.Any(i => i.Id == "lantern"))
            .SetReadText("Meet me at midnight.");
        Location location = new("start");
        location.AddItem(letter);
        GameState state = new(location);

        CommandResult result = state.Execute(new ReadCommand("letter"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotUsable, result.Error);
    }
}

// <copyright file="ReadCommandTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

public class ReadCommandTests
{
    [Fact]
    public void ReadCommand_ReadsReadableItemInLocation()
    {
        var sign = new Item("sign", "Sign")
            .SetTakeable(false)
            .SetReadable()
            .SetReadText("Welcome.");
        var location = new Location("start");
        location.AddItem(sign);
        var state = new GameState(location);

        var result = state.Execute(new ReadCommand("sign"));

        Assert.True(result.Success);
        Assert.Equal("Welcome.", result.Message);
    }

    [Fact]
    public void ReadCommand_RequiresTakeWhenConfigured()
    {
        var paper = new Item("paper", "Paper")
            .SetReadable()
            .RequireTakeToRead()
            .SetReadText("News!");
        var location = new Location("start");
        location.AddItem(paper);
        var state = new GameState(location);

        var result = state.Execute(new ReadCommand("paper"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotInInventory, result.Error);
    }

    [Fact]
    public void ReadCommand_ReadingCostShowsMessage()
    {
        var tome = new Item("tome", "Tome")
            .SetReadable()
            .SetReadingCost(3)
            .SetReadText("Secrets...");
        var location = new Location("start");
        location.AddItem(tome);
        var state = new GameState(location);

        var result = state.Execute(new ReadCommand("tome"));

        Assert.True(result.Success);
        Assert.Contains("3", result.Message);
        Assert.Contains("Secrets", result.Message);
    }

    [Fact]
    public void ReadCommand_RequiresConditionToRead()
    {
        var letter = new Item("letter", "Letter")
            .SetReadable()
            .RequiresToRead(s => s.Inventory.Items.Any(i => i.Id == "lantern"))
            .SetReadText("Meet me at midnight.");
        var location = new Location("start");
        location.AddItem(letter);
        var state = new GameState(location);

        var result = state.Execute(new ReadCommand("letter"));

        Assert.False(result.Success);
        Assert.Equal(GameError.ItemNotUsable, result.Error);
    }
}

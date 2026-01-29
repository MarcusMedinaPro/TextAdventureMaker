// <copyright file="DialogTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Models;

public class DialogTests
{
    [Fact]
    public void DialogNode_StoresTextAndOptions()
    {
        var node = new DialogNode("Hello there.");

        Assert.Equal("Hello there.", node.Text);
        Assert.Empty(node.Options);
    }

    [Fact]
    public void DialogNode_AddOption_WiresNextNode()
    {
        var next = new DialogNode("Second line.");
        var node = new DialogNode("First line.")
            .AddOption("Continue", next);

        _ = Assert.Single(node.Options);
        Assert.Equal("Continue", node.Options[0].Text);
        Assert.Equal(next, node.Options[0].Next);
    }
}

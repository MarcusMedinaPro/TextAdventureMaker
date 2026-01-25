// <copyright file="ExitTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class ExitTests
{
    [Fact]
    public void Exit_IsPassable_WhenNoDoor()
    {
        var target = new Location("target");
        var exit = new Exit(target);

        Assert.True(exit.IsPassable);
    }

    [Fact]
    public void Exit_IsNotPassable_WhenDoorClosed()
    {
        var target = new Location("target");
        var door = new Door("door1", "gate");
        var exit = new Exit(target, door);

        Assert.False(exit.IsPassable);
    }

    [Fact]
    public void Exit_IsPassable_WhenDoorDestroyed()
    {
        var target = new Location("target");
        var door = new Door("door1", "gate");
        door.Destroy();
        var exit = new Exit(target, door);

        Assert.True(exit.IsPassable);
    }

    [Fact]
    public void Exit_NullTarget_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new Exit(null!));
    }
}

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
        Location target = new("target");
        Exit exit = new(target);

        Assert.True(exit.IsPassable);
    }

    [Fact]
    public void Exit_IsNotPassable_WhenDoorClosed()
    {
        Location target = new("target");
        Door door = new("door1", "gate");
        Exit exit = new(target, door);

        Assert.False(exit.IsPassable);
    }

    [Fact]
    public void Exit_IsPassable_WhenDoorDestroyed()
    {
        Location target = new("target");
        Door door = new("door1", "gate");
        _ = door.Destroy();
        Exit exit = new(target, door);

        Assert.True(exit.IsPassable);
    }

    [Fact]
    public void Exit_NullTarget_Throws()
    {
        _ = Assert.Throws<ArgumentNullException>(() => new Exit(null!));
    }
}

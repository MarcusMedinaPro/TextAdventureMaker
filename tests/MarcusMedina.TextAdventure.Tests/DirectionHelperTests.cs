// <copyright file="DirectionHelperTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;

public class DirectionHelperTests
{
    [Theory]
    [InlineData(Direction.North, Direction.South)]
    [InlineData(Direction.South, Direction.North)]
    [InlineData(Direction.East, Direction.West)]
    [InlineData(Direction.West, Direction.East)]
    [InlineData(Direction.Up, Direction.Down)]
    [InlineData(Direction.Down, Direction.Up)]
    [InlineData(Direction.NorthEast, Direction.SouthWest)]
    [InlineData(Direction.NorthWest, Direction.SouthEast)]
    [InlineData(Direction.SouthEast, Direction.NorthWest)]
    [InlineData(Direction.SouthWest, Direction.NorthEast)]
    [InlineData(Direction.In, Direction.Out)]
    [InlineData(Direction.Out, Direction.In)]
    public void GetOpposite_ReturnsExpected(Direction input, Direction expected) => Assert.Equal(expected, DirectionHelper.GetOpposite(input));

    [Fact]
    public void GetOpposite_ThrowsForInvalidDirection()
    {
        var invalid = (Direction)999;
        _ = Assert.Throws<ArgumentOutOfRangeException>(() => DirectionHelper.GetOpposite(invalid));
    }
}

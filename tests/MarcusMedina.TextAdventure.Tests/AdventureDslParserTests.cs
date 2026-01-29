// <copyright file="AdventureDslParserTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class AdventureDslParserTests
{
    [Fact]
    public void ParseFile_BuildsWorldWithExitsAndDoors()
    {
        string file = Path.GetTempFileName();
        try
        {
            File.WriteAllText(file, """
world: Demo World
goal: Find the key
start: entrance

location: entrance | At the gate.
item: ice | ice | A cold chunk. | weight=0.5 | aliases=ice
exit: north -> forest

location: forest | Tall trees.
key: cabin_key | brass key | A small brass key. | weight=0.1 | aliases=key
door: cabin_door | cabin door | A sturdy door. | key=cabin_key
exit: in -> cabin | door=cabin_door

location: cabin | Cozy and quiet.
""");

            AdventureDslParser parser = new();
            DslAdventure adventure = parser.ParseFile(file);

            Assert.Equal("Demo World", adventure.WorldName);
            Assert.Equal("Find the key", adventure.Goal);
            Assert.Equal("entrance", adventure.State.CurrentLocation.Id);

            Location entrance = adventure.Locations["entrance"];
            Location forest = adventure.Locations["forest"];
            Location cabin = adventure.Locations["cabin"];

            Assert.NotNull(entrance.GetExit(Direction.North));
            Assert.Equal(forest, entrance.GetExit(Direction.North)!.Target);

            Exit? forestExit = forest.GetExit(Direction.In);
            Assert.NotNull(forestExit);
            Assert.Equal(cabin, forestExit!.Target);
            Assert.NotNull(forestExit.Door);
            Assert.Equal("cabin_key", forestExit.Door!.RequiredKey!.Id);

            Item ice = adventure.Items["ice"];
            Assert.Equal(0.5f, ice.Weight);
            Assert.Contains("ice", ice.Aliases);
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Fact]
    public void RegisterKeyword_AddsMetadata()
    {
        string file = Path.GetTempFileName();
        try
        {
            File.WriteAllText(file, """
mood: eerie
location: room
""");
            AdventureDslParser parser = new AdventureDslParser()
                .RegisterKeyword("mood", (ctx, value) => ctx.SetMetadata("mood", value));

            DslAdventure adventure = parser.ParseFile(file);

            Assert.Equal("eerie", adventure.GetMetadata("mood"));
        }
        finally
        {
            File.Delete(file);
        }
    }
}

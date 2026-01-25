using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class AdventureDslParserTests
{
    [Fact]
    public void ParseFile_BuildsWorldWithExitsAndDoors()
    {
        var file = Path.GetTempFileName();
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

            var parser = new AdventureDslParser();
            var adventure = parser.ParseFile(file);

            Assert.Equal("Demo World", adventure.WorldName);
            Assert.Equal("Find the key", adventure.Goal);
            Assert.Equal("entrance", adventure.State.CurrentLocation.Id);

            var entrance = adventure.Locations["entrance"];
            var forest = adventure.Locations["forest"];
            var cabin = adventure.Locations["cabin"];

            Assert.NotNull(entrance.GetExit(Direction.North));
            Assert.Equal(forest, entrance.GetExit(Direction.North)!.Target);

            var forestExit = forest.GetExit(Direction.In);
            Assert.NotNull(forestExit);
            Assert.Equal(cabin, forestExit!.Target);
            Assert.NotNull(forestExit.Door);
            Assert.Equal("cabin_key", forestExit.Door!.RequiredKey!.Id);

            var ice = adventure.Items["ice"];
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
        var file = Path.GetTempFileName();
        try
        {
            File.WriteAllText(file, """
mood: eerie
location: room
""");
            var parser = new AdventureDslParser()
                .RegisterKeyword("mood", (ctx, value) => ctx.SetMetadata("mood", value));

            var adventure = parser.ParseFile(file);

            Assert.Equal("eerie", adventure.GetMetadata("mood"));
        }
        finally
        {
            File.Delete(file);
        }
    }
}

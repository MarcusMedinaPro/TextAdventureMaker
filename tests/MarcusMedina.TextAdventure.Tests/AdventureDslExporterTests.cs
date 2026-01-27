// <copyright file="AdventureDslExporterTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Dsl;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class AdventureDslExporterTests
{
    [Fact]
    public void Export_BasicLocation_IncludesLocationLine()
    {
        var location = new Location("test_room", "A test room.");
        var state = new GameState(location);
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state);

        Assert.Contains("location: test_room | A test room.", result);
    }

    [Fact]
    public void Export_WithWorldTitle_IncludesWorldLine()
    {
        var location = new Location("start", "Start room.");
        var state = new GameState(location);
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state, worldTitle: "My Adventure");

        Assert.Contains("world: My Adventure", result);
    }

    [Fact]
    public void Export_WithGoal_IncludesGoalLine()
    {
        var location = new Location("start", "Start room.");
        var state = new GameState(location);
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state, goal: "Find the treasure");

        Assert.Contains("goal: Find the treasure", result);
    }

    [Fact]
    public void Export_WithItem_IncludesItemLine()
    {
        var location = new Location("room", "A room.");
        var item = new Item("sword", "rusty sword", "An old rusty sword.");
        location.AddItem(item);
        var state = new GameState(location);
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state);

        Assert.Contains("item: sword | rusty sword | An old rusty sword.", result);
    }

    [Fact]
    public void Export_WithKey_IncludesKeyLine()
    {
        var location = new Location("room", "A room.");
        var key = new Key("brass_key", "brass key", "A small brass key.");
        location.AddItem(key);
        var state = new GameState(location);
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state);

        Assert.Contains("key: brass_key | brass key | A small brass key.", result);
    }

    [Fact]
    public void Export_WithDoor_IncludesDoorLine()
    {
        var room1 = new Location("room1", "Room one.");
        var room2 = new Location("room2", "Room two.");
        var door = new Door("wooden_door", "wooden door", "A heavy wooden door.");
        room1.AddExit(Direction.North, room2, door);
        var state = new GameState(room1, worldLocations: new[] { room1, room2 });
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state);

        Assert.Contains("door: wooden_door | wooden door | A heavy wooden door.", result);
    }

    [Fact]
    public void Export_WithExit_IncludesExitLine()
    {
        var room1 = new Location("room1", "Room one.");
        var room2 = new Location("room2", "Room two.");
        room1.AddExit(Direction.North, room2);
        var state = new GameState(room1, worldLocations: new[] { room1, room2 });
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state);

        Assert.Contains("exit: north -> room2", result);
    }

    [Fact]
    public void Export_WithDoorOnExit_IncludesDoorOption()
    {
        var room1 = new Location("room1", "Room one.");
        var room2 = new Location("room2", "Room two.");
        var door = new Door("gate", "iron gate", "A rusty iron gate.");
        room1.AddExit(Direction.East, room2, door);
        var state = new GameState(room1, worldLocations: new[] { room1, room2 });
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state);

        Assert.Contains("exit: east -> room2 | door=gate", result);
    }

    [Fact]
    public void Export_WithOneWayExit_IncludesOneWayOption()
    {
        var room1 = new Location("room1", "Room one.");
        var room2 = new Location("room2", "Room two.");
        room1.AddExit(Direction.Down, room2, oneWay: true);
        var state = new GameState(room1, worldLocations: new[] { room1, room2 });
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state);

        Assert.Contains("exit: down -> room2 | oneway", result);
    }

    [Fact]
    public void Export_ItemWithWeight_IncludesWeightOption()
    {
        var location = new Location("room", "A room.");
        var item = new Item("rock", "heavy rock", "A very heavy rock.");
        item.SetWeight(5.5f);
        location.AddItem(item);
        var state = new GameState(location);
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state);

        Assert.Contains("weight=5.5", result);
    }

    [Fact]
    public void Export_NonTakeableItem_IncludesTakeableFalse()
    {
        var location = new Location("room", "A room.");
        var item = new Item("statue", "stone statue", "A heavy stone statue.");
        item.SetTakeable(false);
        location.AddItem(item);
        var state = new GameState(location);
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state);

        Assert.Contains("takeable=false", result);
    }

    [Fact]
    public void Export_DoorWithKey_IncludesKeyOption()
    {
        var room1 = new Location("room1", "Room one.");
        var room2 = new Location("room2", "Room two.");
        var key = new Key("gold_key", "gold key", "A shiny gold key.");
        var door = new Door("vault_door", "vault door", "A massive vault door.");
        door.RequiresKey(key);
        room1.AddItem(key);
        room1.AddExit(Direction.North, room2, door);
        var state = new GameState(room1, worldLocations: new[] { room1, room2 });
        var exporter = new AdventureDslExporter();

        var result = exporter.Export(state);

        Assert.Contains("door: vault_door | vault door | A massive vault door. | key=gold_key", result);
    }

    [Fact]
    public void Export_RoundTrip_ParsedResultMatchesOriginal()
    {
        // Create original state
        var room1 = new Location("entrance", "The entrance hall.");
        var room2 = new Location("garden", "A beautiful garden.");
        var key = new Key("gate_key", "rusty key", "A rusty old key.");
        var door = new Door("garden_gate", "garden gate", "An ornate garden gate.");
        door.RequiresKey(key);
        room1.AddItem(key);
        room1.AddExit(Direction.North, room2, door);

        var originalState = new GameState(room1, worldLocations: new[] { room1, room2 });

        // Export to DSL
        var exporter = new AdventureDslExporter();
        var dslContent = exporter.Export(originalState, "Test World", "Find the garden");

        // Write to temp file and parse back
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, dslContent);
            var parser = new AdventureDslParser();
            var parsed = parser.ParseFile(tempFile);

            // Verify round-trip
            Assert.Equal(2, parsed.Locations.Count);
            Assert.True(parsed.Locations.ContainsKey("entrance"));
            Assert.True(parsed.Locations.ContainsKey("garden"));
            Assert.True(parsed.Keys.ContainsKey("gate_key"));
            Assert.True(parsed.Doors.ContainsKey("garden_gate"));
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}

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
        Location location = new("test_room", "A test room.");
        GameState state = new(location);
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state);

        Assert.Contains("location: test_room | A test room.", result);
    }

    [Fact]
    public void Export_WithWorldTitle_IncludesWorldLine()
    {
        Location location = new("start", "Start room.");
        GameState state = new(location);
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state, worldTitle: "My Adventure");

        Assert.Contains("world: My Adventure", result);
    }

    [Fact]
    public void Export_WithGoal_IncludesGoalLine()
    {
        Location location = new("start", "Start room.");
        GameState state = new(location);
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state, goal: "Find the treasure");

        Assert.Contains("goal: Find the treasure", result);
    }

    [Fact]
    public void Export_WithItem_IncludesItemLine()
    {
        Location location = new("room", "A room.");
        Item item = new("sword", "rusty sword", "An old rusty sword.");
        location.AddItem(item);
        GameState state = new(location);
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state);

        Assert.Contains("item: sword | rusty sword | An old rusty sword.", result);
    }

    [Fact]
    public void Export_WithKey_IncludesKeyLine()
    {
        Location location = new("room", "A room.");
        Key key = new("brass_key", "brass key", "A small brass key.");
        location.AddItem(key);
        GameState state = new(location);
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state);

        Assert.Contains("key: brass_key | brass key | A small brass key.", result);
    }

    [Fact]
    public void Export_WithDoor_IncludesDoorLine()
    {
        Location room1 = new("room1", "Room one.");
        Location room2 = new("room2", "Room two.");
        Door door = new("wooden_door", "wooden door", "A heavy wooden door.");
        _ = room1.AddExit(Direction.North, room2, door);
        GameState state = new(room1, worldLocations: new[] { room1, room2 });
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state);

        Assert.Contains("door: wooden_door | wooden door | A heavy wooden door.", result);
    }

    [Fact]
    public void Export_WithExit_IncludesExitLine()
    {
        Location room1 = new("room1", "Room one.");
        Location room2 = new("room2", "Room two.");
        _ = room1.AddExit(Direction.North, room2);
        GameState state = new(room1, worldLocations: new[] { room1, room2 });
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state);

        Assert.Contains("exit: north -> room2", result);
    }

    [Fact]
    public void Export_WithDoorOnExit_IncludesDoorOption()
    {
        Location room1 = new("room1", "Room one.");
        Location room2 = new("room2", "Room two.");
        Door door = new("gate", "iron gate", "A rusty iron gate.");
        _ = room1.AddExit(Direction.East, room2, door);
        GameState state = new(room1, worldLocations: new[] { room1, room2 });
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state);

        Assert.Contains("exit: east -> room2 | door=gate", result);
    }

    [Fact]
    public void Export_WithOneWayExit_IncludesOneWayOption()
    {
        Location room1 = new("room1", "Room one.");
        Location room2 = new("room2", "Room two.");
        _ = room1.AddExit(Direction.Down, room2, oneWay: true);
        GameState state = new(room1, worldLocations: new[] { room1, room2 });
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state);

        Assert.Contains("exit: down -> room2 | oneway", result);
    }

    [Fact]
    public void Export_ItemWithWeight_IncludesWeightOption()
    {
        Location location = new("room", "A room.");
        Item item = new("rock", "heavy rock", "A very heavy rock.");
        _ = item.SetWeight(5.5f);
        location.AddItem(item);
        GameState state = new(location);
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state);

        Assert.Contains("weight=5.5", result);
    }

    [Fact]
    public void Export_NonTakeableItem_IncludesTakeableFalse()
    {
        Location location = new("room", "A room.");
        Item item = new("statue", "stone statue", "A heavy stone statue.");
        _ = item.SetTakeable(false);
        location.AddItem(item);
        GameState state = new(location);
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state);

        Assert.Contains("takeable=false", result);
    }

    [Fact]
    public void Export_DoorWithKey_IncludesKeyOption()
    {
        Location room1 = new("room1", "Room one.");
        Location room2 = new("room2", "Room two.");
        Key key = new("gold_key", "gold key", "A shiny gold key.");
        Door door = new("vault_door", "vault door", "A massive vault door.");
        _ = door.RequiresKey(key);
        room1.AddItem(key);
        _ = room1.AddExit(Direction.North, room2, door);
        GameState state = new(room1, worldLocations: new[] { room1, room2 });
        AdventureDslExporter exporter = new();

        string result = exporter.Export(state);

        Assert.Contains("door: vault_door | vault door | A massive vault door. | key=gold_key", result);
    }

    [Fact]
    public void Export_RoundTrip_ParsedResultMatchesOriginal()
    {
        // Create original state
        Location room1 = new("entrance", "The entrance hall.");
        Location room2 = new("garden", "A beautiful garden.");
        Key key = new("gate_key", "rusty key", "A rusty old key.");
        Door door = new("garden_gate", "garden gate", "An ornate garden gate.");
        _ = door.RequiresKey(key);
        room1.AddItem(key);
        _ = room1.AddExit(Direction.North, room2, door);

        GameState originalState = new(room1, worldLocations: new[] { room1, room2 });

        // Export to DSL
        AdventureDslExporter exporter = new();
        string dslContent = exporter.Export(originalState, "Test World", "Find the garden");

        // Write to temp file and parse back
        string tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, dslContent);
            AdventureDslParser parser = new();
            DslAdventure parsed = parser.ParseFile(tempFile);

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

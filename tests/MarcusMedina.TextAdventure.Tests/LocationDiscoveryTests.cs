namespace MarcusMedina.TextAdventure.Tests;

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public class LocationDiscoveryTests
{
    [Fact]
    public void Move_DiscoversNewLocation()
    {
        Location start = new("start");
        Location next = new("next");
        _ = start.AddExit(Direction.North, next);

        GameState state = new(start, worldLocations: [start, next]);

        _ = state.Move(Direction.North);

        Assert.True(state.LocationDiscovery.IsDiscovered("next"));
    }

    [Fact]
    public void StartLocation_IsDiscovered()
    {
        Location start = new("start");
        GameState state = new(start);

        Assert.True(state.LocationDiscovery.IsDiscovered("start"));
    }
}


using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class LocationDiscoveryTests
{
    [Fact]
    public void StartLocation_IsDiscovered()
    {
        Location start = new("start");
        GameState state = new(start);

        Assert.True(state.LocationDiscovery.IsDiscovered("start"));
    }

    [Fact]
    public void Move_DiscoversNewLocation()
    {
        Location start = new("start");
        Location next = new("next");
        _ = start.AddExit(Direction.North, next);

        GameState state = new(start, worldLocations: new[] { start, next });

        _ = state.Move(Direction.North);

        Assert.True(state.LocationDiscovery.IsDiscovered("next"));
    }
}

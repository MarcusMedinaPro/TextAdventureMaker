using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;
using Xunit;

namespace MarcusMedina.TextAdventure.Tests;

public class LocationDiscoveryTests
{
    [Fact]
    public void StartLocation_IsDiscovered()
    {
        var start = new Location("start");
        var state = new GameState(start);

        Assert.True(state.LocationDiscovery.IsDiscovered("start"));
    }

    [Fact]
    public void Move_DiscoversNewLocation()
    {
        var start = new Location("start");
        var next = new Location("next");
        start.AddExit(Direction.North, next);

        var state = new GameState(start, worldLocations: new[] { start, next });

        state.Move(Direction.North);

        Assert.True(state.LocationDiscovery.IsDiscovered("next"));
    }
}

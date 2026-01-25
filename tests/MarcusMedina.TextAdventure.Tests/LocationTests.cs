using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class LocationTests
{
    [Fact]
    public void Location_ShouldHaveId()
    {
        var loc = new Location("cave");
        Assert.Equal("cave", loc.Id);
    }

    [Fact]
    public void Location_InvalidId_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Location(""));
        Assert.Throws<ArgumentException>(() => new Location(" "));
        Assert.Throws<ArgumentNullException>(() => new Location(null!));
    }

    [Fact]
    public void Location_ShouldHaveDescription()
    {
        var loc = new Location("cave")
            .Description("A dark cave with glowing mushrooms");

        Assert.Equal("A dark cave with glowing mushrooms", loc.GetDescription());
    }

    [Fact]
    public void AddExit_ShouldCreateBidirectionalPassage()
    {
        var hall = new Location("hall");
        var bedroom = new Location("bedroom");

        hall.AddExit(Direction.North, bedroom);

        Assert.Equal(bedroom, hall.GetExit(Direction.North)?.Target);
        Assert.Equal(hall, bedroom.GetExit(Direction.South)?.Target);
    }

    [Fact]
    public void AddExit_OneWay_ShouldNotCreateReturnPath()
    {
        var hall = new Location("hall");
        var pit = new Location("pit");

        hall.AddExit(Direction.Down, pit, oneWay: true);

        Assert.Equal(pit, hall.GetExit(Direction.Down)?.Target);
        Assert.Null(pit.GetExit(Direction.Up));
    }
}

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class FluidTests
{
    [Fact]
    public void Fluid_CanUseTupleConstructor()
    {
        Fluid water = (id: "water", name: "Water", description: "Clear and cold.");

        Assert.Equal("Water", water.Name);
        Assert.Equal("Clear and cold.", water.GetDescription());
    }
}

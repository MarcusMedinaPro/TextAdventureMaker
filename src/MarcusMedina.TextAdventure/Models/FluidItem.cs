using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class FluidItem : Item, IFluid
{
    public FluidItem(string id, string name) : base(id, name)
    {
    }

    public FluidItem(string id, string name, string description) : base(id, name, description)
    {
    }

    public new FluidItem Description(string text)
    {
        base.Description(text);
        return this;
    }

    IFluid IFluid.Description(string text) => Description(text);

    public static implicit operator FluidItem((string id, string name, string description) data) =>
        new(data.id, data.name, data.description);
}

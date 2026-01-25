using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Glass : ContainerItem<IFluid>
{
    public Glass(string id, string name, int maxCount = 1) : base(id, name, maxCount)
    {
    }

    public Glass(string id, string name, string description, int maxCount = 1) : base(id, name, maxCount)
    {
        Description(description);
    }

    public static implicit operator Glass((string id, string name, string description) data) =>
        new(data.id, data.name, data.description);
}

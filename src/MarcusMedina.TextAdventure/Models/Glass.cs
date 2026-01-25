using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Glass : ContainerItem<IFluid>
{
    public Glass(string id, string name, int maxCount = 1) : base(id, name, maxCount)
    {
    }
}

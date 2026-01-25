using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Chest : ContainerItem<IItem>
{
    public Chest(string id, string name, int maxCount = 0) : base(id, name, maxCount)
    {
    }
}

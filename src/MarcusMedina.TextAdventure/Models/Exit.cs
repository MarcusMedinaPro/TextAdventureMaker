using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Exit
{
    public ILocation Target { get; }
    public IDoor? Door { get; }
    public bool IsOneWay { get; }

    public bool IsPassable => Door == null || Door.IsPassable;

    public Exit(ILocation target, IDoor? door = null, bool isOneWay = false)
    {
        ArgumentNullException.ThrowIfNull(target);
        Target = target;
        Door = door;
        IsOneWay = isOneWay;
    }
}

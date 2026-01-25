using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Key : IKey
{
    public string Id { get; }
    public string Name { get; }

    public Key(string id, string name)
    {
        Id = id;
        Name = name;
    }
}

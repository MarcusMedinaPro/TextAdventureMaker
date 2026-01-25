namespace MarcusMedina.TextAdventure.Interfaces;

public interface IGameEntity
{
    string Id { get; }
    string Name { get; }
    IDictionary<string, string> Properties { get; }
}

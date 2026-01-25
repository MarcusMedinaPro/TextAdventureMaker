using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ISaveSystem
{
    void Save(string path, GameMemento memento);
    GameMemento Load(string path);
}

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ILocation
{
    string Id { get; }
    string GetDescription();
    Exit? GetExit(Direction direction);
    IReadOnlyDictionary<Direction, Exit> Exits { get; }
    IReadOnlyList<IItem> Items { get; }
    IReadOnlyList<INpc> Npcs { get; }
    void AddItem(IItem item);
    bool RemoveItem(IItem item);
    IItem? FindItem(string name);
    void AddNpc(INpc npc);
    bool RemoveNpc(INpc npc);
    INpc? FindNpc(string name);
}

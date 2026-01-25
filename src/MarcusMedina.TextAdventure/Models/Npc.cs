using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Npc : INpc
{
    private string _description = "";

    public string Id { get; }
    public string Name { get; }
    public NpcState State { get; private set; }
    public bool IsAlive => State != NpcState.Dead;

    public Npc(string id, string name, NpcState state = NpcState.Friendly)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = id;
        Name = name;
        State = state;
    }

    public string GetDescription() => _description;

    public INpc Description(string text)
    {
        _description = text ?? "";
        return this;
    }

    public INpc SetState(NpcState state)
    {
        State = state;
        return this;
    }
}

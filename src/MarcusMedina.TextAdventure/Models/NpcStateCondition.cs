using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class NpcStateCondition : IQuestCondition
{
    public INpc Npc { get; }
    public NpcState RequiredState { get; }

    public NpcStateCondition(INpc npc, NpcState requiredState)
    {
        ArgumentNullException.ThrowIfNull(npc);
        Npc = npc;
        RequiredState = requiredState;
    }

    public bool Accept(IQuestConditionVisitor visitor) => visitor.Visit(this);
}

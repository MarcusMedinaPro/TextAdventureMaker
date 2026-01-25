using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class RelationshipCondition : IQuestCondition
{
    public string NpcId { get; }
    public int Minimum { get; }

    public RelationshipCondition(string npcId, int minimum)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(npcId);
        NpcId = npcId;
        Minimum = minimum;
    }

    public bool Accept(IQuestConditionVisitor visitor) => visitor.Visit(this);
}

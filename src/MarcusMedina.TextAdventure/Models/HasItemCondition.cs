using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class HasItemCondition : IQuestCondition
{
    public string ItemId { get; }

    public HasItemCondition(string itemId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        ItemId = itemId;
    }

    public bool Accept(IQuestConditionVisitor visitor) => visitor.Visit(this);
}

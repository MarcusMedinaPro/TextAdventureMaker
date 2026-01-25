using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class AnyOfCondition : IQuestCondition
{
    private readonly List<IQuestCondition> _conditions;
    public IReadOnlyList<IQuestCondition> Conditions => _conditions;

    public AnyOfCondition(IEnumerable<IQuestCondition> conditions)
    {
        ArgumentNullException.ThrowIfNull(conditions);
        _conditions = conditions.Where(c => c != null).ToList();
    }

    public bool Accept(IQuestConditionVisitor visitor) => visitor.Visit(this);
}

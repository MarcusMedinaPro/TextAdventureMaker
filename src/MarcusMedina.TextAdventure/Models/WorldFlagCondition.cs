using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class WorldFlagCondition : IQuestCondition
{
    public string Key { get; }
    public bool Expected { get; }

    public WorldFlagCondition(string key, bool expected = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        Key = key;
        Expected = expected;
    }

    public bool Accept(IQuestConditionVisitor visitor) => visitor.Visit(this);
}

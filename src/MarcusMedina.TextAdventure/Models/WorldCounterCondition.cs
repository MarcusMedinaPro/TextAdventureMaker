using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class WorldCounterCondition : IQuestCondition
{
    public string Key { get; }
    public int Minimum { get; }

    public WorldCounterCondition(string key, int minimum)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        Key = key;
        Minimum = minimum;
    }

    public bool Accept(IQuestConditionVisitor visitor) => visitor.Visit(this);
}

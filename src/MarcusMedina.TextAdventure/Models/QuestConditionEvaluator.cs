using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using System.Linq;

namespace MarcusMedina.TextAdventure.Models;

public sealed class QuestConditionEvaluator : IQuestConditionVisitor
{
    private readonly IGameState _state;

    public QuestConditionEvaluator(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        _state = state;
    }

    public bool Visit(HasItemCondition condition)
    {
        return _state.Inventory.Items.Any(item => item.Id.TextCompare(condition.ItemId));
    }

    public bool Visit(NpcStateCondition condition)
    {
        return condition.Npc.State == condition.RequiredState;
    }

    public bool Visit(AllOfCondition condition)
    {
        return condition.Conditions.All(child => child.Accept(this));
    }

    public bool Visit(AnyOfCondition condition)
    {
        return condition.Conditions.Any(child => child.Accept(this));
    }

    public bool Visit(WorldFlagCondition condition)
    {
        return _state.WorldState.GetFlag(condition.Key) == condition.Expected;
    }

    public bool Visit(WorldCounterCondition condition)
    {
        return _state.WorldState.GetCounter(condition.Key) >= condition.Minimum;
    }

    public bool Visit(RelationshipCondition condition)
    {
        return _state.WorldState.GetRelationship(condition.NpcId) >= condition.Minimum;
    }
}

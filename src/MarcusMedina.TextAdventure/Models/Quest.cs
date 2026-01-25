using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Quest : IQuest
{
    private readonly List<IQuestCondition> _conditions = new();

    public string Id { get; }
    public string Title { get; }
    public string Description { get; }
    public QuestState State { get; private set; }
    public IReadOnlyList<IQuestCondition> Conditions => _conditions;

    public Quest(string id, string title, string description, QuestState state = QuestState.Inactive)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Id = id;
        Title = title;
        Description = description;
        State = state;
    }

    public IQuest Start()
    {
        if (State == QuestState.Inactive)
        {
            State = QuestState.Active;
        }
        return this;
    }

    public IQuest Complete()
    {
        State = QuestState.Completed;
        return this;
    }

    public IQuest Fail()
    {
        State = QuestState.Failed;
        return this;
    }

    public IQuest AddCondition(IQuestCondition condition)
    {
        ArgumentNullException.ThrowIfNull(condition);
        _conditions.Add(condition);
        return this;
    }

    public bool CheckProgress(IGameState state)
    {
        if (State != QuestState.Active || _conditions.Count == 0)
        {
            return false;
        }

        var evaluator = new QuestConditionEvaluator(state);
        if (_conditions.All(condition => condition.Accept(evaluator)))
        {
            State = QuestState.Completed;
            return true;
        }

        return false;
    }
}

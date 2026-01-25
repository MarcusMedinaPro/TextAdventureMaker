using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Quest : IQuest
{
    public string Id { get; }
    public string Title { get; }
    public string Description { get; }
    public QuestState State { get; private set; }

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
}

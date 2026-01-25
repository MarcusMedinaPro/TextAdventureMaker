using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IQuest
{
    string Id { get; }
    string Title { get; }
    string Description { get; }
    QuestState State { get; }

    IQuest Start();
    IQuest Complete();
    IQuest Fail();
}

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IWorldState
{
    bool GetFlag(string key);
    void SetFlag(string key, bool value);

    int GetCounter(string key);
    int Increment(string key, int amount = 1);

    int GetRelationship(string npcId);
    void SetRelationship(string npcId, int value);

    IReadOnlyList<string> Timeline { get; }
    void AddTimeline(string entry);
}

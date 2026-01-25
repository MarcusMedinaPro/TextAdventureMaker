namespace MarcusMedina.TextAdventure.Interfaces;

public interface IItem
{
    string Id { get; }
    string Name { get; }
    string GetDescription();
    bool Takeable { get; }
    float Weight { get; }
    IReadOnlyList<string> Aliases { get; }

    event Action<IItem>? OnTake;
    event Action<IItem>? OnDrop;
    event Action<IItem>? OnUse;
    event Action<IItem>? OnDestroy;

    bool Matches(string name);
    IItem Description(string text);
    void Take();
    void Drop();
    void Use();
    void Destroy();
}

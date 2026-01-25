using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public abstract class ItemDecorator : IItem
{
    protected IItem Inner { get; }

    protected ItemDecorator(IItem inner)
    {
        Inner = inner;
    }

    public virtual string Id => Inner.Id;
    public virtual string Name => Inner.Name;
    public virtual bool Takeable => Inner.Takeable;
    public virtual float Weight => Inner.Weight;
    public virtual IReadOnlyList<string> Aliases => Inner.Aliases;

    public virtual string GetDescription() => Inner.GetDescription();
    public virtual IItem Description(string text) => Inner.Description(text);

    public event Action<IItem>? OnTake
    {
        add => Inner.OnTake += value;
        remove => Inner.OnTake -= value;
    }

    public event Action<IItem>? OnDrop
    {
        add => Inner.OnDrop += value;
        remove => Inner.OnDrop -= value;
    }

    public event Action<IItem>? OnUse
    {
        add => Inner.OnUse += value;
        remove => Inner.OnUse -= value;
    }

    public event Action<IItem>? OnDestroy
    {
        add => Inner.OnDestroy += value;
        remove => Inner.OnDestroy -= value;
    }

    public virtual bool Matches(string name) => Inner.Matches(name);
    public virtual void Take() => Inner.Take();
    public virtual void Drop() => Inner.Drop();
    public virtual void Use() => Inner.Use();
    public virtual void Destroy() => Inner.Destroy();
}

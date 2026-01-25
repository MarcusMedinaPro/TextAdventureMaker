using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Item : IItem
{
    private readonly List<string> _aliases = new();
    private readonly Dictionary<ItemAction, string> _reactions = new();
    private string _description = "";

    public string Id { get; }
    public string Name { get; }
    public string GetDescription() => _description;
    public bool Takeable { get; private set; }
    public float Weight { get; private set; }
    public IReadOnlyList<string> Aliases => _aliases;

    public event Action<IItem>? OnTake;
    public event Action<IItem>? OnDrop;
    public event Action<IItem>? OnUse;
    public event Action<IItem>? OnDestroy;

    public Item(string id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = id;
        Name = name;
        Takeable = true;
        Weight = 0f;
    }

    public Item SetTakeable(bool takeable)
    {
        Takeable = takeable;
        return this;
    }

    public Item SetWeight(float weight)
    {
        Weight = Math.Max(0f, weight);
        return this;
    }

    public IItem Description(string text)
    {
        _description = text;
        return this;
    }

    public Item AddAliases(params string[] aliases)
    {
        foreach (var alias in aliases)
        {
            if (!string.IsNullOrWhiteSpace(alias))
            {
                _aliases.Add(alias.Trim());
            }
        }

        return this;
    }

    public bool Matches(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var token = name.Trim();

        if (Name.TextCompare(token)) return true;
        return _aliases.Any(a => a.TextCompare(token));
    }

    public string? GetReaction(ItemAction action)
    {
        return _reactions.TryGetValue(action, out var reaction) ? reaction : null;
    }

    public IItem SetReaction(ItemAction action, string text)
    {
        _reactions[action] = text;
        return this;
    }

    public void Take()
    {
        OnTake?.Invoke(this);
    }

    public void Drop()
    {
        OnDrop?.Invoke(this);
    }

    public void Use()
    {
        OnUse?.Invoke(this);
    }

    public void Destroy()
    {
        OnDestroy?.Invoke(this);
    }

    public static implicit operator Item(string name) =>
        new(name.ToId(), name);
}

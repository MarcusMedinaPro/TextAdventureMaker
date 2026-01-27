// <copyright file="Item.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Item : IItem
{
    private readonly List<string> _aliases = new();
    private readonly Dictionary<ItemAction, string> _reactions = new();
    private readonly Dictionary<string, string> _properties = new(StringComparer.OrdinalIgnoreCase);
    private string? _readText;
    private bool _readable;
    private bool _requiresTakeToRead;
    private int _readingCost;
    private Func<IGameState, bool>? _readCondition;
    private bool _hiddenFromItemList;
    private string _description = "";

    public string Id { get; }
    public string Name { get; }
    public IDictionary<string, string> Properties => _properties;
    public string GetDescription() => _description;
    public bool Takeable { get; private set; }
    public float Weight { get; private set; }
    public IReadOnlyList<string> Aliases => _aliases;
    public bool Readable => _readable;
    public bool RequiresTakeToRead => _requiresTakeToRead;
    public int ReadingCost => _readingCost;
    public bool HiddenFromItemList => _hiddenFromItemList;

    public event Action<IItem>? OnTake;
    public event Action<IItem>? OnDrop;
    public event Action<IItem>? OnUse;
    public event Action<IItem>? OnMove;
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

    public Item(string id, string name, string description) : this(id, name)
    {
        _description = description ?? "";
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

    public bool CanRead(IGameState state)
    {
        return _readCondition == null || _readCondition(state);
    }

    public IItem SetReadable(bool readable = true)
    {
        _readable = readable;
        return this;
    }

    public IItem SetReadText(string text)
    {
        _readText = text;
        _readable = true;
        return this;
    }

    public string? GetReadText() => _readText;

    public IItem RequireTakeToRead()
    {
        _requiresTakeToRead = true;
        return this;
    }

    public IItem SetReadingCost(int turns)
    {
        _readingCost = Math.Max(0, turns);
        return this;
    }

    public IItem HideFromItemList(bool hidden = true)
    {
        _hiddenFromItemList = hidden;
        return this;
    }

    public IItem RequiresToRead(Func<IGameState, bool> predicate)
    {
        _readCondition = predicate ?? throw new ArgumentNullException(nameof(predicate));
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

    public void Move()
    {
        OnMove?.Invoke(this);
    }

    public void Destroy()
    {
        OnDestroy?.Invoke(this);
    }

    public static implicit operator Item(string name) =>
        new(name.ToId(), name);

    public static implicit operator Item((string id, string name, string description) data) =>
        new(data.id, data.name, data.description);
}

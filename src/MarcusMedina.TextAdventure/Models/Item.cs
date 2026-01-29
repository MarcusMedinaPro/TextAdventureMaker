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
    private readonly List<string> _aliases = [];
    private readonly Dictionary<ItemAction, string> _reactions = [];
    private readonly Dictionary<string, string> _properties = new(StringComparer.OrdinalIgnoreCase);
    private string? _readText;
    private Func<IGameState, bool>? _readCondition;
    private string _description = "";

    public string Id { get; }
    public string Name { get; }
    public IDictionary<string, string> Properties => _properties;
    public string GetDescription()
    {
        return _description;
    }

    public bool Takeable { get; private set; }
    public float Weight { get; private set; }
    public IReadOnlyList<string> Aliases => _aliases;
    public bool Readable { get; private set; }
    public bool RequiresTakeToRead { get; private set; }
    public int ReadingCost { get; private set; }
    public bool HiddenFromItemList { get; private set; }

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
        foreach (string alias in aliases)
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
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        string token = name.Trim();

        return Name.TextCompare(token) || _aliases.Any(a => a.TextCompare(token));
    }

    public string? GetReaction(ItemAction action)
    {
        return _reactions.TryGetValue(action, out string? reaction) ? reaction : null;
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
        Readable = readable;
        return this;
    }

    public IItem SetReadText(string text)
    {
        _readText = text;
        Readable = true;
        return this;
    }

    public string? GetReadText()
    {
        return _readText;
    }

    public IItem RequireTakeToRead()
    {
        RequiresTakeToRead = true;
        return this;
    }

    public IItem SetReadingCost(int turns)
    {
        ReadingCost = Math.Max(0, turns);
        return this;
    }

    public IItem HideFromItemList(bool hidden = true)
    {
        HiddenFromItemList = hidden;
        return this;
    }

    public IItem RequiresToRead(Func<IGameState, bool> predicate)
    {
        _readCondition = predicate ?? throw new ArgumentNullException(nameof(predicate));
        return this;
    }

    public virtual IItem Clone()
    {
        IItem copy = new Item(Id, Name, _description)
            .SetTakeable(Takeable)
            .SetWeight(Weight)
            .SetReadable(Readable)
            .SetReadingCost(ReadingCost)
            .HideFromItemList(HiddenFromItemList);

        if (_aliases.Count > 0)
        {
            _ = copy.AddAliases(_aliases.ToArray());
        }

        if (_readText != null)
        {
            _ = copy.SetReadText(_readText);
        }

        if (RequiresTakeToRead)
        {
            _ = copy.RequireTakeToRead();
        }

        if (_readCondition != null)
        {
            _ = copy.RequiresToRead(_readCondition);
        }

        foreach (KeyValuePair<ItemAction, string> reaction in _reactions)
        {
            _ = copy.SetReaction(reaction.Key, reaction.Value);
        }

        foreach (KeyValuePair<string, string> entry in _properties)
        {
            copy.Properties[entry.Key] = entry.Value;
        }

        return copy;
    }

    IItem IItem.SetTakeable(bool takeable)
    {
        return SetTakeable(takeable);
    }

    IItem IItem.SetWeight(float weight)
    {
        return SetWeight(weight);
    }

    IItem IItem.AddAliases(params string[] aliases)
    {
        return AddAliases(aliases);
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

    public static implicit operator Item(string name)
    {
        return new(name.ToId(), name);
    }

    public static implicit operator Item((string id, string name, string description) data)
    {
        return new(data.id, data.name, data.description);
    }
}

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
    private int? _amount;
    private bool _isStackable;
    private string? _presenceDescription;
    private bool _isFood;
    private bool _isPoisoned;
    private int _healAmount;

    public string Id { get; }
    public string Name { get; }
    public string? Description => _description;
    public int? Amount => _amount;
    public bool IsStackable => _isStackable;
    public string? PresenceDescription => _presenceDescription;
    public bool IsFood => _isFood;
    public bool IsPoisoned => _isPoisoned;
    public int HealAmount => _healAmount;
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
    public event Action<IItem>? OnAmountEmpty;

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

    public Item SetDescription(string text)
    {
        _description = text;
        return this;
    }

    public Item SetAmount(int amount)
    {
        _amount = Math.Max(0, amount);
        return this;
    }

    public bool DecreaseAmount(int amount = 1)
    {
        if (!_amount.HasValue)
        {
            return true;
        }

        int next = Math.Max(0, _amount.Value - Math.Max(1, amount));
        _amount = next;

        if (next == 0)
        {
            OnAmountEmpty?.Invoke(this);
            return false;
        }

        return true;
    }

    public Item SetStackable(bool isStackable = true)
    {
        _isStackable = isStackable;
        return this;
    }

    public Item SetPresenceDescription(string text)
    {
        _presenceDescription = text;
        return this;
    }

    public Item SetFood(bool isFood = true)
    {
        _isFood = isFood;
        return this;
    }

    public Item SetPoisoned(bool isPoisoned = true)
    {
        _isPoisoned = isPoisoned;
        return this;
    }

    public Item SetHealAmount(int amount)
    {
        _healAmount = Math.Max(0, amount);
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

    // Explicit interface implementations for IItem fluent methods
    IItem IItem.SetTakeable(bool takeable) => SetTakeable(takeable);
    IItem IItem.SetWeight(float weight) => SetWeight(weight);
    IItem IItem.SetDescription(string description) => SetDescription(description);
    IItem IItem.SetAmount(int amount) => SetAmount(amount);
    bool IItem.DecreaseAmount(int amount) => DecreaseAmount(amount);
    IItem IItem.SetStackable(bool isStackable) => SetStackable(isStackable);
    IItem IItem.SetPresenceDescription(string text) => SetPresenceDescription(text);
    IItem IItem.SetFood(bool isFood) => SetFood(isFood);
    IItem IItem.SetPoisoned(bool isPoisoned) => SetPoisoned(isPoisoned);
    IItem IItem.SetHealAmount(int amount) => SetHealAmount(amount);
    IItem IItem.AddAliases(params string[] aliases) => AddAliases(aliases);

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
            .SetStackable(_isStackable)
            .SetReadable(Readable)
            .SetReadingCost(ReadingCost)
            .HideFromItemList(HiddenFromItemList);

        if (_amount.HasValue)
        {
            _ = copy.SetAmount(_amount.Value);
        }

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

        if (!string.IsNullOrWhiteSpace(_presenceDescription))
        {
            _ = copy.SetPresenceDescription(_presenceDescription);
        }

        if (_isFood)
        {
            _ = copy.SetFood();
        }

        if (_isPoisoned)
        {
            _ = copy.SetPoisoned();
        }

        if (_healAmount > 0)
        {
            _ = copy.SetHealAmount(_healAmount);
        }

        return copy;
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
        _ = DecreaseAmount();
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

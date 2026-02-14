// <copyright file="Door.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Door(string id, string name, DoorState initialState = DoorState.Closed) : IDoor
{
    public string Id { get; } = ValidateId(id);
    public string Name { get; } = ValidateName(name);
    public IDictionary<string, string> Properties => _properties;
    public IReadOnlyList<string> Aliases => _aliases;
    public DoorState State { get; private set; } = initialState;
    public IKey? RequiredKey { get; private set; }

    private string _description = "";
    private readonly Dictionary<DoorAction, string> _reactions = [];
    private readonly Dictionary<string, string> _properties = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _aliases = [];

    public event Action<IDoor>? OnOpen;
    public event Action<IDoor>? OnClose;
    public event Action<IDoor>? OnLock;
    public event Action<IDoor>? OnUnlock;
    public event Action<IDoor>? OnDestroy;

    public bool IsPassable => State is DoorState.Open or DoorState.Destroyed;

    public Door(string id, string name, string description, DoorState initialState = DoorState.Closed)
        : this(id, name, initialState)
    {
        _description = description ?? "";
    }

    private static string ValidateId(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        return id;
    }

    private static string ValidateName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return name;
    }

    public Door Description(string text)
    {
        _description = text;
        return this;
    }

    public string GetDescription() => _description;

    public string? GetReaction(DoorAction action) =>
        _reactions.TryGetValue(action, out string? reaction) ? reaction : null;

    public Door AddAliases(params string[] aliases)
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
            return false;

        string token = name.Trim();
        return Name.TextCompare(token) || _aliases.Any(a => a.TextCompare(token));
    }

    public Door SetReaction(DoorAction action, string text)
    {
        _reactions[action] = text;
        return this;
    }

    public Door RequiresKey(IKey key)
    {
        RequiredKey = key;
        State = DoorState.Locked;
        return this;
    }

    public bool Open() => State switch
    {
        DoorState.Locked => false,
        DoorState.Destroyed or DoorState.Open => true,
        _ => ChangeState(DoorState.Open, OnOpen)
    };

    private bool ChangeState(DoorState newState, Action<IDoor>? onStateChange)
    {
        State = newState;
        onStateChange?.Invoke(this);
        return true;
    }

    public bool Close() => State switch
    {
        DoorState.Destroyed or DoorState.Locked => false,
        DoorState.Closed => true,
        _ => ChangeState(DoorState.Closed, OnClose)
    };

    public bool Lock(IKey key)
    {
        if (State == DoorState.Destroyed || State == DoorState.Open)
            return false;

        if (RequiredKey != null && RequiredKey.Id != key.Id)
            return false;

        if (State == DoorState.Locked && RequiredKey?.Id == key.Id)
            return true;

        RequiredKey = key;
        return ChangeState(DoorState.Locked, OnLock);
    }

    public bool Unlock(IKey key)
    {
        if (State != DoorState.Locked)
            return false;

        if (RequiredKey == null || RequiredKey.Id != key.Id)
            return false;

        return ChangeState(DoorState.Closed, OnUnlock);
    }

    public bool Destroy() => State == DoorState.Destroyed ? true : ChangeState(DoorState.Destroyed, OnDestroy);

    public static implicit operator Door(string name) => new(name.ToId(), name);

    public static implicit operator Door((string id, string name, string description) data) =>
        new(data.id, data.name, data.description);

    IDoor IDoor.Description(string text) => Description(text);

    IDoor IDoor.AddAliases(params string[] aliases) => AddAliases(aliases);

    bool IDoor.Matches(string name) => Matches(name);

    IDoor IDoor.SetReaction(DoorAction action, string text) => SetReaction(action, text);
}

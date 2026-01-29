// <copyright file="Door.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Door : IDoor
{
    private string _description = "";
    private readonly Dictionary<DoorAction, string> _reactions = [];
    private readonly Dictionary<string, string> _properties = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _aliases = [];
    public string Id { get; }
    public string Name { get; }
    public IDictionary<string, string> Properties => _properties;
    public IReadOnlyList<string> Aliases => _aliases;
    public string GetDescription()
    {
        return _description;
    }

    public DoorState State { get; private set; }
    public IKey? RequiredKey { get; private set; }

    public event Action<IDoor>? OnOpen;
    public event Action<IDoor>? OnClose;
    public event Action<IDoor>? OnLock;
    public event Action<IDoor>? OnUnlock;
    public event Action<IDoor>? OnDestroy;

    public bool IsPassable => State is DoorState.Open or DoorState.Destroyed;

    public Door(string id, string name, DoorState initialState = DoorState.Closed)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = id;
        Name = name;
        State = initialState;
    }

    public Door(string id, string name, string description, DoorState initialState = DoorState.Closed)
        : this(id, name, initialState)
    {
        _description = description ?? "";
    }

    public Door Description(string text)
    {
        _description = text;
        return this;
    }

    public string? GetReaction(DoorAction action)
    {
        return _reactions.TryGetValue(action, out string? reaction) ? reaction : null;
    }

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
        {
            return false;
        }

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

    public bool Open()
    {
        if (State == DoorState.Locked)
        {
            return false;
        }

        if (State == DoorState.Destroyed)
        {
            return true;
        }

        if (State == DoorState.Open)
        {
            return true;
        }

        State = DoorState.Open;
        OnOpen?.Invoke(this);
        return true;
    }

    public bool Close()
    {
        if (State == DoorState.Destroyed)
        {
            return false;
        }

        if (State == DoorState.Locked)
        {
            return false;
        }

        if (State == DoorState.Closed)
        {
            return true;
        }

        State = DoorState.Closed;
        OnClose?.Invoke(this);
        return true;
    }

    public bool Lock(IKey key)
    {
        if (State == DoorState.Destroyed)
        {
            return false;
        }

        if (State == DoorState.Open)
        {
            return false;
        }

        if (RequiredKey != null && RequiredKey.Id != key.Id)
        {
            return false;
        }

        if (State == DoorState.Locked && RequiredKey != null && RequiredKey.Id == key.Id)
        {
            return true;
        }

        RequiredKey = key;
        State = DoorState.Locked;
        OnLock?.Invoke(this);
        return true;
    }

    public bool Unlock(IKey key)
    {
        if (State != DoorState.Locked)
        {
            return false;
        }

        if (RequiredKey == null || RequiredKey.Id != key.Id)
        {
            return false;
        }

        State = DoorState.Closed;
        OnUnlock?.Invoke(this);
        return true;
    }

    public bool Destroy()
    {
        if (State == DoorState.Destroyed)
        {
            return true;
        }

        State = DoorState.Destroyed;
        OnDestroy?.Invoke(this);
        return true;
    }

    public static implicit operator Door(string name)
    {
        return new(name.ToId(), name);
    }

    IDoor IDoor.Description(string text)
    {
        return Description(text);
    }

    IDoor IDoor.AddAliases(params string[] aliases)
    {
        return AddAliases(aliases);
    }

    bool IDoor.Matches(string name)
    {
        return Matches(name);
    }

    IDoor IDoor.SetReaction(DoorAction action, string text)
    {
        return SetReaction(action, text);
    }

    public static implicit operator Door((string id, string name, string description) data)
    {
        return new(data.id, data.name, data.description);
    }
}

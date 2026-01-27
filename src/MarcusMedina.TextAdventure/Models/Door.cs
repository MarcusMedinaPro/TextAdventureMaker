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
    private readonly Dictionary<DoorAction, string> _reactions = new();
    private readonly Dictionary<string, string> _properties = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _aliases = new();
    public string Id { get; }
    public string Name { get; }
    public IDictionary<string, string> Properties => _properties;
    public IReadOnlyList<string> Aliases => _aliases;
    public string GetDescription() => _description;
    public DoorState State { get; private set; }
    public IKey? RequiredKey { get; private set; }

    public bool IsPassable => State == DoorState.Open || State == DoorState.Destroyed;

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
        return _reactions.TryGetValue(action, out var reaction) ? reaction : null;
    }

    public Door AddAliases(params string[] aliases)
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
        if (State == DoorState.Locked) return false;
        if (State == DoorState.Destroyed) return true;
        State = DoorState.Open;
        return true;
    }

    public bool Close()
    {
        if (State == DoorState.Destroyed) return false;
        if (State == DoorState.Locked) return false;
        State = DoorState.Closed;
        return true;
    }

    public bool Lock(IKey key)
    {
        if (State == DoorState.Destroyed) return false;
        if (State == DoorState.Open) return false;
        if (RequiredKey != null && RequiredKey.Id != key.Id) return false;
        RequiredKey = key;
        State = DoorState.Locked;
        return true;
    }

    public bool Unlock(IKey key)
    {
        if (State != DoorState.Locked) return false;
        if (RequiredKey == null || RequiredKey.Id != key.Id) return false;
        State = DoorState.Closed;
        return true;
    }

    public bool Destroy()
    {
        State = DoorState.Destroyed;
        return true;
    }

    public static implicit operator Door(string name) => new(name.ToId(), name);

    IDoor IDoor.Description(string text) => Description(text);
    IDoor IDoor.AddAliases(params string[] aliases) => AddAliases(aliases);
    bool IDoor.Matches(string name) => Matches(name);
    IDoor IDoor.SetReaction(DoorAction action, string text) => SetReaction(action, text);

    public static implicit operator Door((string id, string name, string description) data) =>
        new(data.id, data.name, data.description);
}

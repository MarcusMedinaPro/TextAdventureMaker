// <copyright file="DoorBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Builders;

public sealed class DoorBuilder
{
    private readonly Door _door;

    private DoorBuilder(Door door)
    {
        _door = door;
    }

    public static DoorBuilder Create(string id, string name, string description = "", DoorState initialState = DoorState.Closed)
    {
        var door = string.IsNullOrWhiteSpace(description)
            ? new Door(id, name, initialState)
            : new Door(id, name, description, initialState);
        return new DoorBuilder(door);
    }

    public DoorBuilder Description(string text)
    {
        _door.Description(text);
        return this;
    }

    public DoorBuilder RequiresKey(Key key)
    {
        _door.RequiresKey(key);
        return this;
    }

    public DoorBuilder SetReaction(DoorAction action, string text)
    {
        _door.SetReaction(action, text);
        return this;
    }

    public DoorBuilder AddAliases(params string[] aliases)
    {
        _door.AddAliases(aliases);
        return this;
    }

    public Door Build() => _door;
}

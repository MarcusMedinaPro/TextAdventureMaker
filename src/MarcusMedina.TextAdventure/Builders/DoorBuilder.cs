// <copyright file="DoorBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Builders;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public sealed class DoorBuilder
{
    private readonly Door _door;

    private DoorBuilder(Door door) => _door = door;

    public static DoorBuilder Create(string id, string name, string description = "", DoorState initialState = DoorState.Closed)
    {
        var door = string.IsNullOrWhiteSpace(description)
            ? new Door(id, name, initialState)
            : new Door(id, name, description, initialState);
        return new DoorBuilder(door);
    }

    public DoorBuilder Description(string text)
    {
        _ = _door.Description(text);
        return this;
    }

    public DoorBuilder RequiresKey(Key key)
    {
        _ = _door.RequiresKey(key);
        return this;
    }

    public DoorBuilder SetReaction(DoorAction action, string text)
    {
        _ = _door.SetReaction(action, text);
        return this;
    }

    public DoorBuilder AddAliases(params string[] aliases)
    {
        _ = _door.AddAliases(aliases);
        return this;
    }

    public Door Build() => _door;
}

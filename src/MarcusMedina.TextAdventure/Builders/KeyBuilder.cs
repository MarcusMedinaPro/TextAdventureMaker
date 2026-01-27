// <copyright file="KeyBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Builders;

public sealed class KeyBuilder
{
    private readonly Key _key;

    private KeyBuilder(Key key)
    {
        _key = key;
    }

    public static KeyBuilder Create(string id, string name, string description = "")
    {
        var key = string.IsNullOrWhiteSpace(description)
            ? new Key(id, name)
            : new Key(id, name, description);
        return new KeyBuilder(key);
    }

    public KeyBuilder Description(string text)
    {
        _key.Description(text);
        return this;
    }

    public KeyBuilder SetWeight(float weight)
    {
        _key.SetWeight(weight);
        return this;
    }

    public KeyBuilder SetTakeable(bool takeable = true)
    {
        _key.SetTakeable(takeable);
        return this;
    }

    public KeyBuilder AddAliases(params string[] aliases)
    {
        _key.AddAliases(aliases);
        return this;
    }

    public KeyBuilder SetReadable(bool readable = true)
    {
        _key.SetReadable(readable);
        return this;
    }

    public KeyBuilder SetReadText(string text)
    {
        _key.SetReadText(text);
        return this;
    }

    public KeyBuilder RequireTakeToRead()
    {
        _key.RequireTakeToRead();
        return this;
    }

    public KeyBuilder SetReadingCost(int turns)
    {
        _key.SetReadingCost(turns);
        return this;
    }

    public KeyBuilder RequiresToRead(Func<IGameState, bool> predicate)
    {
        _key.RequiresToRead(predicate);
        return this;
    }

    public KeyBuilder SetReaction(ItemAction action, string text)
    {
        _key.SetReaction(action, text);
        return this;
    }

    public Key Build() => _key;
}

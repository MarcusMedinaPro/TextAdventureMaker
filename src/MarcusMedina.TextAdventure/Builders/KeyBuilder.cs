// <copyright file="KeyBuilder.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Builders;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

public sealed class KeyBuilder
{
    private readonly Key _key;

    private KeyBuilder(Key key) => _key = key;

    public static KeyBuilder Create(string id, string name, string description = "")
    {
        var key = string.IsNullOrWhiteSpace(description)
            ? new Key(id, name)
            : new Key(id, name, description);
        return new KeyBuilder(key);
    }

    public KeyBuilder Description(string text)
    {
        _ = _key.Description(text);
        return this;
    }

    public KeyBuilder SetWeight(float weight)
    {
        _ = _key.SetWeight(weight);
        return this;
    }

    public KeyBuilder SetTakeable(bool takeable = true)
    {
        _ = _key.SetTakeable(takeable);
        return this;
    }

    public KeyBuilder AddAliases(params string[] aliases)
    {
        _ = _key.AddAliases(aliases);
        return this;
    }

    public KeyBuilder SetReadable(bool readable = true)
    {
        _ = _key.SetReadable(readable);
        return this;
    }

    public KeyBuilder SetReadText(string text)
    {
        _ = _key.SetReadText(text);
        return this;
    }

    public KeyBuilder RequireTakeToRead()
    {
        _ = _key.RequireTakeToRead();
        return this;
    }

    public KeyBuilder SetReadingCost(int turns)
    {
        _ = _key.SetReadingCost(turns);
        return this;
    }

    public KeyBuilder RequiresToRead(Func<IGameState, bool> predicate)
    {
        _ = _key.RequiresToRead(predicate);
        return this;
    }

    public KeyBuilder SetReaction(ItemAction action, string text)
    {
        _ = _key.SetReaction(action, text);
        return this;
    }

    public Key Build() => _key;
}

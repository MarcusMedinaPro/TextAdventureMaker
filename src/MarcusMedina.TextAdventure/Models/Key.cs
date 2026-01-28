// <copyright file="Key.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;
using System.Linq;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Key : Item, IKey
{
    public Key(string id, string name) : base(id, name)
    {
    }

    public Key(string id, string name, string description) : base(id, name, description)
    {
    }

    public new Key SetTakeable(bool takeable)
    {
        base.SetTakeable(takeable);
        return this;
    }

    public new Key SetWeight(float weight)
    {
        base.SetWeight(weight);
        return this;
    }

    public new Key AddAliases(params string[] aliases)
    {
        base.AddAliases(aliases);
        return this;
    }

    public new Key Description(string text)
    {
        base.Description(text);
        return this;
    }

    public new Key SetReaction(ItemAction action, string text)
    {
        base.SetReaction(action, text);
        return this;
    }

    public new Key SetReadable(bool readable = true)
    {
        base.SetReadable(readable);
        return this;
    }

    public new Key SetReadText(string text)
    {
        base.SetReadText(text);
        return this;
    }

    public new Key RequireTakeToRead()
    {
        base.RequireTakeToRead();
        return this;
    }

    public new Key SetReadingCost(int turns)
    {
        base.SetReadingCost(turns);
        return this;
    }

    public new Key RequiresToRead(Func<IGameState, bool> predicate)
    {
        base.RequiresToRead(predicate);
        return this;
    }

    public override IItem Clone()
    {
        var copy = new Key(Id, Name, GetDescription())
            .SetTakeable(Takeable)
            .SetWeight(Weight)
            .SetReadable(Readable)
            .SetReadingCost(ReadingCost)
            .HideFromItemList(HiddenFromItemList);

        if (Aliases.Count > 0)
        {
            copy.AddAliases(Aliases.ToArray());
        }

        foreach (ItemAction action in Enum.GetValues(typeof(ItemAction)))
        {
            var reaction = GetReaction(action);
            if (!string.IsNullOrWhiteSpace(reaction))
            {
                copy.SetReaction(action, reaction);
            }
        }

        foreach (var entry in Properties)
        {
            copy.Properties[entry.Key] = entry.Value;
        }

        var readText = GetReadText();
        if (readText != null)
        {
            copy.SetReadText(readText);
        }

        if (RequiresTakeToRead)
        {
            copy.RequireTakeToRead();
        }

        return copy;
    }

    public static implicit operator Key((string id, string name, string description) data) =>
        new(data.id, data.name, data.description);
}

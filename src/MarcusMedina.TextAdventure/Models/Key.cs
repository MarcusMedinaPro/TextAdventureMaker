// <copyright file="Key.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using System;
using System.Linq;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

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
        _ = base.SetTakeable(takeable);
        return this;
    }

    public new Key SetWeight(float weight)
    {
        _ = base.SetWeight(weight);
        return this;
    }

    public new Key AddAliases(params string[] aliases)
    {
        _ = base.AddAliases(aliases);
        return this;
    }

    public new Key Description(string text)
    {
        _ = base.Description(text);
        return this;
    }

    public new Key SetReaction(ItemAction action, string text)
    {
        _ = base.SetReaction(action, text);
        return this;
    }

    public new Key SetReadable(bool readable = true)
    {
        _ = base.SetReadable(readable);
        return this;
    }

    public new Key SetReadText(string text)
    {
        _ = base.SetReadText(text);
        return this;
    }

    public new Key RequireTakeToRead()
    {
        _ = base.RequireTakeToRead();
        return this;
    }

    public new Key SetReadingCost(int turns)
    {
        _ = base.SetReadingCost(turns);
        return this;
    }

    public new Key RequiresToRead(Func<IGameState, bool> predicate)
    {
        _ = base.RequiresToRead(predicate);
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
            _ = copy.AddAliases(Aliases.ToArray());
        }

        foreach (ItemAction action in Enum.GetValues(typeof(ItemAction)))
        {
            var reaction = GetReaction(action);
            if (!string.IsNullOrWhiteSpace(reaction))
            {
                _ = copy.SetReaction(action, reaction);
            }
        }

        foreach (var entry in Properties)
        {
            copy.Properties[entry.Key] = entry.Value;
        }

        var readText = GetReadText();
        if (readText != null)
        {
            _ = copy.SetReadText(readText);
        }

        if (RequiresTakeToRead)
        {
            _ = copy.RequireTakeToRead();
        }

        return copy;
    }

    public static implicit operator Key((string id, string name, string description) data) =>
        new(data.id, data.name, data.description);
}

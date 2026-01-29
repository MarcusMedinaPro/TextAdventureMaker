// <copyright file="ItemDecorator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public abstract class ItemDecorator : IItem
{
    protected IItem Inner { get; }

    protected ItemDecorator(IItem inner)
    {
        Inner = inner;
    }

    public virtual string Id => Inner.Id;
    public virtual string Name => Inner.Name;
    public virtual IDictionary<string, string> Properties => Inner.Properties;
    public virtual bool Takeable => Inner.Takeable;
    public virtual float Weight => Inner.Weight;
    public virtual IReadOnlyList<string> Aliases => Inner.Aliases;
    public virtual bool Readable => Inner.Readable;
    public virtual bool RequiresTakeToRead => Inner.RequiresTakeToRead;
    public virtual int ReadingCost => Inner.ReadingCost;
    public virtual bool HiddenFromItemList => Inner.HiddenFromItemList;

    public virtual string GetDescription()
    {
        return Inner.GetDescription();
    }

    public virtual IItem SetTakeable(bool takeable)
    {
        return Inner.SetTakeable(takeable);
    }

    public virtual IItem SetWeight(float weight)
    {
        return Inner.SetWeight(weight);
    }

    public virtual IItem Description(string text)
    {
        return Inner.Description(text);
    }

    public virtual IItem AddAliases(params string[] aliases)
    {
        return Inner.AddAliases(aliases);
    }

    public virtual string? GetReadText()
    {
        return Inner.GetReadText();
    }

    public event Action<IItem>? OnTake
    {
        add => Inner.OnTake += value;
        remove => Inner.OnTake -= value;
    }

    public event Action<IItem>? OnDrop
    {
        add => Inner.OnDrop += value;
        remove => Inner.OnDrop -= value;
    }

    public event Action<IItem>? OnUse
    {
        add => Inner.OnUse += value;
        remove => Inner.OnUse -= value;
    }

    public event Action<IItem>? OnMove
    {
        add => Inner.OnMove += value;
        remove => Inner.OnMove -= value;
    }

    public event Action<IItem>? OnDestroy
    {
        add => Inner.OnDestroy += value;
        remove => Inner.OnDestroy -= value;
    }

    public virtual bool Matches(string name)
    {
        return Inner.Matches(name);
    }

    public virtual string? GetReaction(ItemAction action)
    {
        return Inner.GetReaction(action);
    }

    public virtual IItem SetReaction(ItemAction action, string text)
    {
        return Inner.SetReaction(action, text);
    }

    public virtual bool CanRead(IGameState state)
    {
        return Inner.CanRead(state);
    }

    public virtual IItem SetReadable(bool readable = true)
    {
        return Inner.SetReadable(readable);
    }

    public virtual IItem SetReadText(string text)
    {
        return Inner.SetReadText(text);
    }

    public virtual IItem RequireTakeToRead()
    {
        return Inner.RequireTakeToRead();
    }

    public virtual IItem SetReadingCost(int turns)
    {
        return Inner.SetReadingCost(turns);
    }

    public virtual IItem HideFromItemList(bool hidden = true)
    {
        return Inner.HideFromItemList(hidden);
    }

    public virtual IItem RequiresToRead(Func<IGameState, bool> predicate)
    {
        return Inner.RequiresToRead(predicate);
    }

    public virtual IItem Clone()
    {
        return Inner.Clone();
    }

    public virtual void Take()
    {
        Inner.Take();
    }

    public virtual void Drop()
    {
        Inner.Drop();
    }

    public virtual void Use()
    {
        Inner.Use();
    }

    public virtual void Move()
    {
        Inner.Move();
    }

    public virtual void Destroy()
    {
        Inner.Destroy();
    }
}

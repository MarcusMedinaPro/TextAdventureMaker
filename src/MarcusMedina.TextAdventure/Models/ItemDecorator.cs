// <copyright file="ItemDecorator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

public abstract class ItemDecorator(IItem inner) : IItem
{

    public event Action<IItem>? OnDestroy
    {
        add => Inner.OnDestroy += value;
        remove => Inner.OnDestroy -= value;
    }

    public event Action<IItem>? OnDrop
    {
        add => Inner.OnDrop += value;
        remove => Inner.OnDrop -= value;
    }

    public event Action<IItem>? OnMove
    {
        add => Inner.OnMove += value;
        remove => Inner.OnMove -= value;
    }

    public event Action<IItem>? OnTake
    {
        add => Inner.OnTake += value;
        remove => Inner.OnTake -= value;
    }

    public event Action<IItem>? OnUse
    {
        add => Inner.OnUse += value;
        remove => Inner.OnUse -= value;
    }

    public virtual IReadOnlyList<string> Aliases => Inner.Aliases;
    public virtual bool HiddenFromItemList => Inner.HiddenFromItemList;
    public virtual string Id => Inner.Id;
    public virtual string Name => Inner.Name;
    public virtual IDictionary<string, string> Properties => Inner.Properties;
    public virtual bool Readable => Inner.Readable;
    public virtual int ReadingCost => Inner.ReadingCost;
    public virtual bool RequiresTakeToRead => Inner.RequiresTakeToRead;
    public virtual bool Takeable => Inner.Takeable;
    public virtual float Weight => Inner.Weight;
    protected IItem Inner { get; } = inner;

    public virtual IItem AddAliases(params string[] aliases) => Inner.AddAliases(aliases);

    public virtual bool CanRead(IGameState state) => Inner.CanRead(state);

    public virtual IItem Clone() => Inner.Clone();

    public virtual IItem Description(string text) => Inner.Description(text);

    public virtual void Destroy() => Inner.Destroy();

    public virtual void Drop() => Inner.Drop();

    public virtual string GetDescription() => Inner.GetDescription();

    public virtual string? GetReaction(ItemAction action) => Inner.GetReaction(action);

    public virtual string? GetReadText() => Inner.GetReadText();

    public virtual IItem HideFromItemList(bool hidden = true) => Inner.HideFromItemList(hidden);

    public virtual bool Matches(string name) => Inner.Matches(name);

    public virtual void Move() => Inner.Move();

    public virtual IItem RequiresToRead(Func<IGameState, bool> predicate) => Inner.RequiresToRead(predicate);

    public virtual IItem RequireTakeToRead() => Inner.RequireTakeToRead();

    public virtual IItem SetReaction(ItemAction action, string text) => Inner.SetReaction(action, text);

    public virtual IItem SetReadable(bool readable = true) => Inner.SetReadable(readable);

    public virtual IItem SetReadingCost(int turns) => Inner.SetReadingCost(turns);

    public virtual IItem SetReadText(string text) => Inner.SetReadText(text);

    public virtual IItem SetTakeable(bool takeable) => Inner.SetTakeable(takeable);

    public virtual IItem SetWeight(float weight) => Inner.SetWeight(weight);

    public virtual void Take() => Inner.Take();

    public virtual void Use() => Inner.Use();
}

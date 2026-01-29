// <copyright file="DialogRule.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using System;
using System.Collections.Generic;
using System.Linq;

public sealed class DialogRule
{
    private readonly List<Func<DialogContext, bool>> _conditions = [];
    private Func<DialogContext, string>? _say;
    private Action<DialogContext>? _then;

    public string Id { get; }
    public int PriorityValue { get; private set; }
    public int CriteriaCount => _conditions.Count;

    public DialogRule(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
    }

    public DialogRule When(Func<DialogContext, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        _conditions.Add(predicate);
        return this;
    }

    public DialogRule Say(string text)
    {
        _say = _ => text ?? "";
        return this;
    }

    public DialogRule Say(Func<DialogContext, string> say)
    {
        ArgumentNullException.ThrowIfNull(say);
        _say = say;
        return this;
    }

    public DialogRule Then(Action<DialogContext> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        _then = action;
        return this;
    }

    public DialogRule Priority(int priority)
    {
        PriorityValue = priority;
        return this;
    }

    public bool Matches(DialogContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return _conditions.All(condition => condition(context));
    }

    public string? GetText(DialogContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return _say?.Invoke(context);
    }

    public void Apply(DialogContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _then?.Invoke(context);
    }
}

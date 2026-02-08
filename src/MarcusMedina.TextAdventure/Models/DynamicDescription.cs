// <copyright file="DynamicDescription.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class DynamicDescription
{
    private readonly List<(Func<IGameState, bool> Condition, string Text)> _conditions = [];
    private string _default = "";
    private string? _firstVisit;
    private readonly Dictionary<string, string> _variables = new(StringComparer.OrdinalIgnoreCase);

    public DynamicDescription Default(string text)
    {
        _default = text ?? "";
        return this;
    }

    public DynamicDescription FirstVisit(string text)
    {
        _firstVisit = text ?? "";
        return this;
    }

    public DynamicDescription When(Func<IGameState, bool> predicate, string text)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        _conditions.Add((predicate, text ?? ""));
        return this;
    }

    public DynamicDescription WithVariable(string key, string value)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            _variables[key] = value ?? "";
        }

        return this;
    }

    public string Resolve(IGameState state, bool firstVisit)
    {
        string? text = firstVisit && !string.IsNullOrWhiteSpace(_firstVisit)
            ? _firstVisit
            : _conditions.FirstOrDefault(c => c.Condition(state)).Text ?? _default;

        if (string.IsNullOrWhiteSpace(text))
        {
            text = _default;
        }

        return ApplyVariables(text ?? "");
    }

    private string ApplyVariables(string text)
    {
        foreach ((string key, string value) in _variables)
        {
            text = text.Replace("{" + key + "}", value, StringComparison.OrdinalIgnoreCase);
        }

        return text;
    }
}

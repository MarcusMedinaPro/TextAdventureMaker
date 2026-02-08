// <copyright file="LayeredDescription.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class LayeredDescription
{
    private string? _default;
    private string? _firstVisit;
    private string? _secondVisit;
    private string? _thirdVisit;
    private readonly Dictionary<string, string> _itemOverrides = new(StringComparer.OrdinalIgnoreCase);

    public LayeredDescription Default(string text)
    {
        _default = text ?? "";
        return this;
    }

    public LayeredDescription FirstVisit(string text)
    {
        _firstVisit = text ?? "";
        return this;
    }

    public LayeredDescription SecondVisit(string text)
    {
        _secondVisit = text ?? "";
        return this;
    }

    public LayeredDescription ThirdVisit(string text)
    {
        _thirdVisit = text ?? "";
        return this;
    }

    public LayeredDescription OnItem(string itemId, string text)
    {
        if (!string.IsNullOrWhiteSpace(itemId))
        {
            _itemOverrides[itemId] = text ?? "";
        }

        return this;
    }

    public string Resolve(IGameState state, int visitCount)
    {
        if (state != null)
        {
            foreach ((string itemId, string text) in _itemOverrides)
            {
                if (state.Inventory.Items.Any(item => item.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase)))
                {
                    return text;
                }
            }
        }

        string? text = visitCount switch
        {
            0 => _firstVisit,
            1 => _secondVisit,
            _ => _thirdVisit
        };

        if (string.IsNullOrWhiteSpace(text))
        {
            text = _default;
        }

        return text ?? "";
    }
}

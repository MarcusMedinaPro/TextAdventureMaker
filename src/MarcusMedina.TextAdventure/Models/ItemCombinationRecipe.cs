// <copyright file="ItemCombinationRecipe.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public class ItemCombinationRecipe
{
    public string LeftId { get; }
    public string RightId { get; }
    public Func<IItem> Create { get; }

    public ItemCombinationRecipe(string leftId, string rightId, Func<IItem> create)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(leftId);
        ArgumentException.ThrowIfNullOrWhiteSpace(rightId);
        Create = create ?? throw new ArgumentNullException(nameof(create));
        LeftId = leftId;
        RightId = rightId;
    }

    public bool Matches(IItem a, IItem b) => (a.Id == LeftId && b.Id == RightId) || (a.Id == RightId && b.Id == LeftId);
}

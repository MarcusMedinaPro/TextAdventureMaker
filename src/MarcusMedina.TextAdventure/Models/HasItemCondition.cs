// <copyright file="HasItemCondition.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class HasItemCondition : IQuestCondition
{
    public string ItemId { get; }

    public HasItemCondition(string itemId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        ItemId = itemId;
    }

    public bool Accept(IQuestConditionVisitor visitor)
    {
        return visitor.Visit(this);
    }
}

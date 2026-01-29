// <copyright file="AllOfCondition.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class AllOfCondition : IQuestCondition
{
    private readonly List<IQuestCondition> _conditions;
    public IReadOnlyList<IQuestCondition> Conditions => _conditions;

    public AllOfCondition(IEnumerable<IQuestCondition> conditions)
    {
        ArgumentNullException.ThrowIfNull(conditions);
        _conditions = conditions.Where(c => c != null).ToList();
    }

    public bool Accept(IQuestConditionVisitor visitor)
    {
        return visitor.Visit(this);
    }
}

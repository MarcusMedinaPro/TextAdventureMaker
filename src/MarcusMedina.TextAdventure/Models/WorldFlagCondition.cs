// <copyright file="WorldFlagCondition.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class WorldFlagCondition : IQuestCondition
{
    public string Key { get; }
    public bool Expected { get; }

    public WorldFlagCondition(string key, bool expected = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        Key = key;
        Expected = expected;
    }

    public bool Accept(IQuestConditionVisitor visitor)
    {
        return visitor.Visit(this);
    }
}

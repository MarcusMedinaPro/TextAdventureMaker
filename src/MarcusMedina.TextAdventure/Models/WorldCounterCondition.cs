// <copyright file="WorldCounterCondition.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public sealed class WorldCounterCondition : IQuestCondition
{
    public string Key { get; }
    public int Minimum { get; }

    public WorldCounterCondition(string key, int minimum)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        Key = key;
        Minimum = minimum;
    }

    public bool Accept(IQuestConditionVisitor visitor) => visitor.Visit(this);
}

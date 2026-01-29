// <copyright file="RelationshipCondition.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using MarcusMedina.TextAdventure.Interfaces;

public sealed class RelationshipCondition : IQuestCondition
{
    public string NpcId { get; }
    public int Minimum { get; }

    public RelationshipCondition(string npcId, int minimum)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(npcId);
        NpcId = npcId;
        Minimum = minimum;
    }

    public bool Accept(IQuestConditionVisitor visitor) => visitor.Visit(this);
}

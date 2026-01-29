// <copyright file="NpcStateCondition.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class NpcStateCondition : IQuestCondition
{
    public INpc Npc { get; }
    public NpcState RequiredState { get; }

    public NpcStateCondition(INpc npc, NpcState requiredState)
    {
        ArgumentNullException.ThrowIfNull(npc);
        Npc = npc;
        RequiredState = requiredState;
    }

    public bool Accept(IQuestConditionVisitor visitor)
    {
        return visitor.Visit(this);
    }
}

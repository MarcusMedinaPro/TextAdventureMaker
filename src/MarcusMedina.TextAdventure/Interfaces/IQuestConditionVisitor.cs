// <copyright file="IQuestConditionVisitor.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IQuestConditionVisitor
{
    bool Visit(HasItemCondition condition);
    bool Visit(NpcStateCondition condition);
    bool Visit(AllOfCondition condition);
    bool Visit(AnyOfCondition condition);
    bool Visit(WorldFlagCondition condition);
    bool Visit(WorldCounterCondition condition);
    bool Visit(RelationshipCondition condition);
}

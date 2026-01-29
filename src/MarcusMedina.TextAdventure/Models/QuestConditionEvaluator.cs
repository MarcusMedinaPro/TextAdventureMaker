// <copyright file="QuestConditionEvaluator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using System.Linq;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

public sealed class QuestConditionEvaluator : IQuestConditionVisitor
{
    private readonly IGameState _state;

    public QuestConditionEvaluator(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        _state = state;
    }

    public bool Visit(HasItemCondition condition) => _state.Inventory.Items.Any(item => item.Id.TextCompare(condition.ItemId));

    public bool Visit(NpcStateCondition condition) => condition.Npc.State == condition.RequiredState;

    public bool Visit(AllOfCondition condition) => condition.Conditions.All(child => child.Accept(this));

    public bool Visit(AnyOfCondition condition) => condition.Conditions.Any(child => child.Accept(this));

    public bool Visit(WorldFlagCondition condition) => _state.WorldState.GetFlag(condition.Key) == condition.Expected;

    public bool Visit(WorldCounterCondition condition) => _state.WorldState.GetCounter(condition.Key) >= condition.Minimum;

    public bool Visit(RelationshipCondition condition) => _state.WorldState.GetRelationship(condition.NpcId) >= condition.Minimum;
}

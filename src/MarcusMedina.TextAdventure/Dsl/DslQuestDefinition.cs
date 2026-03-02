// <copyright file="DslQuestDefinition.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 quest definition models (Slice 079).
/// </summary>

/// <summary>
/// Quest definition for DSL v2.
/// </summary>
public sealed class DslQuest
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string State { get; set; } = "active"; // active, complete, failed, abandoned
}

/// <summary>
/// Quest stage definition for DSL v2.
/// </summary>
public sealed class DslQuestStage
{
    public string QuestId { get; set; } = "";
    public string StageId { get; set; } = "";
    public List<string> Required { get; set; } = []; // Required objectives
    public List<string> Optional { get; set; } = []; // Optional objectives
}

/// <summary>
/// Quest objective definition for DSL v2.
/// </summary>
public sealed class DslQuestObjective
{
    public string QuestId { get; set; } = "";
    public string ObjectiveId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
}

/// <summary>
/// Quest condition for DSL v2.
/// </summary>
public sealed class DslQuestCondition
{
    public string QuestId { get; set; } = "";
    public string Expression { get; set; } = ""; // Condition expression like "has_item:sword AND counter:kills>=5"
}

/// <summary>
/// Quest lifecycle effect for DSL v2.
/// </summary>
public sealed class DslQuestOnComplete
{
    public string QuestId { get; set; } = "";
    public string Effects { get; set; } = ""; // Shared effect grammar
}

/// <summary>
/// Quest failure effect for DSL v2.
/// </summary>
public sealed class DslQuestOnFail
{
    public string QuestId { get; set; } = "";
    public string Effects { get; set; } = "";
}

/// <summary>
/// Condition evaluator for quest conditions.
/// </summary>
public sealed class DslQuestConditionEvaluator
{
    /// <summary>
    /// Evaluate a condition expression against game state.
    /// Returns true if condition is met.
    /// </summary>
    public bool Evaluate(string expression, DslQuestEvaluationContext context)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return false;

        expression = expression.ToLowerInvariant();

        // Handle AND/OR operators
        if (expression.Contains(" and "))
        {
            var parts = expression.Split(" and ");
            return parts.All(p => Evaluate(p.Trim(), context));
        }

        if (expression.Contains(" or "))
        {
            var parts = expression.Split(" or ");
            return parts.Any(p => Evaluate(p.Trim(), context));
        }

        // Handle has_item:id
        if (expression.StartsWith("has_item:"))
        {
            string itemId = expression[9..];
            return context.InventoryItems.Contains(itemId);
        }

        // Handle flag:key=value
        if (expression.StartsWith("flag:"))
        {
            var kv = expression[5..].Split('=');
            if (kv.Length == 2 && context.Flags.TryGetValue(kv[0], out var value))
                return value.ToString().Equals(kv[1], StringComparison.OrdinalIgnoreCase);
            return false;
        }

        // Handle counter:key>=value or counter:key=value
        if (expression.StartsWith("counter:"))
        {
            return EvaluateComparison(expression[8..], context.Counters);
        }

        // Handle relationship:npc_id>=value
        if (expression.StartsWith("relationship:"))
        {
            return EvaluateComparison(expression[13..], context.Relationships);
        }

        // Handle npc_state:npc_id=state
        if (expression.StartsWith("npc_state:"))
        {
            var kv = expression[10..].Split('=');
            if (kv.Length == 2 && context.NpcStates.TryGetValue(kv[0], out var state))
                return state.Equals(kv[1], StringComparison.OrdinalIgnoreCase);
            return false;
        }

        return false;
    }

    private bool EvaluateComparison(string expr, Dictionary<string, int> values)
    {
        // Supports: key>=5, key>5, key<=5, key<5, key=5, key!=5
        foreach (var op in new[] { ">=", "<=", "!=", "=", ">", "<" })
        {
            if (expr.Contains(op))
            {
                var parts = expr.Split(op, 2);
                if (parts.Length == 2 &&
                    values.TryGetValue(parts[0].Trim(), out var value) &&
                    int.TryParse(parts[1].Trim(), out var target))
                {
                    return op switch
                    {
                        ">=" => value >= target,
                        "<=" => value <= target,
                        ">" => value > target,
                        "<" => value < target,
                        "=" => value == target,
                        "!=" => value != target,
                        _ => false
                    };
                }
            }
        }
        return false;
    }
}

/// <summary>
/// Evaluation context for quest conditions.
/// </summary>
public sealed class DslQuestEvaluationContext
{
    public List<string> InventoryItems { get; set; } = [];
    public Dictionary<string, bool> Flags { get; set; } = [];
    public Dictionary<string, int> Counters { get; set; } = [];
    public Dictionary<string, int> Relationships { get; set; } = [];
    public Dictionary<string, string> NpcStates { get; set; } = [];
}

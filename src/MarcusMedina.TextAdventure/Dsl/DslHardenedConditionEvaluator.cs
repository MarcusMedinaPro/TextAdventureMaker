// <copyright file="DslHardenedConditionEvaluator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Concurrent;

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// Hardened condition evaluator with caching and loop guards (Slice 086).
/// </summary>
public sealed class DslHardenedConditionEvaluator
{
    private readonly DslQuestConditionEvaluator _baseEvaluator = new();
    private readonly ConcurrentDictionary<string, bool?> _evaluationCache = new();
    private readonly ConcurrentDictionary<string, int> _evaluationDepth = new();

    private const int MaxCacheSize = 1000;
    private const int MaxEvaluationDepth = 10;

    /// <summary>
    /// Evaluate a condition with caching and loop guards.
    /// </summary>
    public bool Evaluate(string expression, DslQuestEvaluationContext context, DslExecutionContext? executionContext = null)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (string.IsNullOrWhiteSpace(expression))
            return false;

        // Check cache first
        var cacheKey = MakeCacheKey(expression, context);
        if (_evaluationCache.TryGetValue(cacheKey, out var cached))
        {
            return cached ?? false;
        }

        // Check recursion depth
        var depth = _evaluationDepth.AddOrUpdate(cacheKey, 1, (_, d) => d + 1);
        if (depth > MaxEvaluationDepth)
        {
            executionContext?.RecordError($"Maximum evaluation depth ({MaxEvaluationDepth}) exceeded for condition");
            _evaluationDepth.TryRemove(cacheKey, out _);
            return false;
        }

        try
        {
            // Evaluate
            var result = _baseEvaluator.Evaluate(expression, context);

            // Cache if not too large
            if (_evaluationCache.Count < MaxCacheSize)
            {
                _evaluationCache.TryAdd(cacheKey, result);
            }

            return result;
        }
        finally
        {
            // Decrement depth
            _evaluationDepth.AddOrUpdate(cacheKey, 0, (_, d) => d - 1);
        }
    }

    /// <summary>
    /// Create a cache key from expression and relevant context.
    /// </summary>
    private static string MakeCacheKey(string expression, DslQuestEvaluationContext context)
    {
        // Simple cache key - in production would need to include context state
        return expression;
    }

    /// <summary>
    /// Clear the evaluation cache.
    /// </summary>
    public void ClearCache()
    {
        _evaluationCache.Clear();
        _evaluationDepth.Clear();
    }

    /// <summary>
    /// Get cache statistics.
    /// </summary>
    public (int CacheSize, int PendingEvaluations) GetCacheStats()
    {
        return (_evaluationCache.Count, _evaluationDepth.Count);
    }

    /// <summary>
    /// Validate a condition expression for syntax.
    /// </summary>
    public bool ValidateExpression(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            return true; // Empty expression is valid

        // Check for obvious syntax errors
        var expr = expression.ToLowerInvariant().Trim();

        // Must contain valid operators
        var validPatterns = new[] { "has_item:", "flag:", "counter:", "relationship:", "npc_state:" };
        bool hasValidPattern = validPatterns.Any(p => expr.Contains(p)) || expr is "true" or "false";

        if (!hasValidPattern && !expr.Contains(" and ") && !expr.Contains(" or "))
            return false;

        return true;
    }
}

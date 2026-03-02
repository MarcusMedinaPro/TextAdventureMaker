// <copyright file="DslHardenedEffectExecutor.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Concurrent;

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// Hardened effect executor with caching, loop guards, and deterministic execution (Slice 086).
/// </summary>
public sealed class DslHardenedEffectExecutor
{
    private readonly DslEffectExecutor _baseExecutor = new();
    private readonly ConcurrentDictionary<string, List<DslEffect>> _compiledEffectCache = new();
    private readonly ConcurrentDictionary<string, int> _triggerInvocationCount = new();
    private readonly object _lockObj = new();

    private const int MaxTriggerInvocationsPerTick = 100;
    private const int MaxCacheSize = 1000;

    /// <summary>
    /// Execute effects with loop guards and caching.
    /// </summary>
    public void Execute(string effectString, DslExecutionContext context, string? triggerSourceId = null)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Check recursion depth
        if (context.IsRecursionLimitExceeded())
        {
            context.RecordError($"Maximum recursion depth ({context.MaxRecursionDepth}) exceeded");
            return;
        }

        // Check trigger invocation count
        if (!string.IsNullOrEmpty(triggerSourceId))
        {
            lock (_lockObj)
            {
                _triggerInvocationCount.TryGetValue(triggerSourceId, out var count);
                if (count >= MaxTriggerInvocationsPerTick)
                {
                    context.RecordError($"Trigger {triggerSourceId} invoked too many times ({count}) this tick");
                    return;
                }
                _triggerInvocationCount[triggerSourceId] = count + 1;
            }
        }

        context.RecursionDepth++;

        try
        {
            // Get or compile effects
            var effects = GetOrCompileEffects(effectString);

            // Execute in deterministic order
            foreach (var effect in effects)
            {
                if (context.StopOnError && context.HasError)
                    break;

                ExecuteEffect(effect, context);
            }
        }
        finally
        {
            context.RecursionDepth--;
        }
    }

    /// <summary>
    /// Get cached compiled effects or compile them.
    /// </summary>
    private List<DslEffect> GetOrCompileEffects(string effectString)
    {
        if (string.IsNullOrWhiteSpace(effectString))
            return [];

        // Check cache
        if (_compiledEffectCache.TryGetValue(effectString, out var cached))
            return cached;

        // Compile (parse and cache)
        var effects = _baseExecutor.ParseEffects(effectString).ToList();

        // Add to cache if not too large
        if (_compiledEffectCache.Count < MaxCacheSize)
        {
            _compiledEffectCache.TryAdd(effectString, effects);
        }

        return effects;
    }

    /// <summary>
    /// Execute a single effect.
    /// </summary>
    private void ExecuteEffect(DslEffect effect, DslExecutionContext context)
    {
        // Validate effect
        if (!_baseExecutor.ValidateEffect(effect))
        {
            context.RecordWarning($"Invalid effect: {effect.Type}");
            return;
        }

        try
        {
            // Execute based on type
            ExecuteEffectByType(effect, context);
            context.RecordInfo($"Executed effect: {effect.Type}");
        }
        catch (Exception ex)
        {
            context.RecordError($"Effect execution failed: {effect.Type} - {ex.Message}");
            if (context.StopOnError)
                throw;
        }
    }

    /// <summary>
    /// Execute effect based on its type.
    /// </summary>
    private static void ExecuteEffectByType(DslEffect effect, DslExecutionContext context)
    {
        switch (effect.Type)
        {
            case "spawn_item":
                ExecuteSpawnItem(effect, context);
                break;
            case "spawn_npc":
                ExecuteSpawnNpc(effect, context);
                break;
            case "open_door":
                ExecuteOpenDoor(effect, context);
                break;
            case "move_npc":
                ExecuteMoveNpc(effect, context);
                break;
            case "message":
                ExecuteMessage(effect, context);
                break;
            default:
                context.RecordWarning($"Unknown effect type: {effect.Type}");
                break;
        }
    }

    private static void ExecuteSpawnItem(DslEffect effect, DslExecutionContext context)
    {
        // Spawn item at location
        // effect.Param1 = item ID, effect.Param2 = location ID
        if (string.IsNullOrEmpty(effect.Param1) || string.IsNullOrEmpty(effect.Param2))
        {
            context.RecordError("spawn_item requires item_id and location_id");
            return;
        }

        var location = context.GameState.Locations.FirstOrDefault(l => l.Id == effect.Param2);
        if (location is null)
        {
            context.RecordWarning($"Location not found: {effect.Param2}");
            return;
        }

        context.RecordInfo($"Spawning item {effect.Param1} at {effect.Param2}");
        // Actual item spawning would be handled by game engine
    }

    private static void ExecuteSpawnNpc(DslEffect effect, DslExecutionContext context)
    {
        // Spawn NPC at location
        // effect.Param1 = NPC ID, effect.Param2 = location ID
        if (string.IsNullOrEmpty(effect.Param1) || string.IsNullOrEmpty(effect.Param2))
        {
            context.RecordError("spawn_npc requires npc_id and location_id");
            return;
        }

        var location = context.GameState.Locations.FirstOrDefault(l => l.Id == effect.Param2);
        if (location is null)
        {
            context.RecordWarning($"Location not found: {effect.Param2}");
            return;
        }

        context.RecordInfo($"Spawning NPC {effect.Param1} at {effect.Param2}");
        // Actual NPC spawning would be handled by game engine
    }

    private static void ExecuteOpenDoor(DslEffect effect, DslExecutionContext context)
    {
        // Open a door
        // effect.Param1 = door ID
        if (string.IsNullOrEmpty(effect.Param1))
        {
            context.RecordError("open_door requires door_id");
            return;
        }

        context.RecordInfo($"Opening door {effect.Param1}");
        // Actual door opening would be handled by game engine
    }

    private static void ExecuteMoveNpc(DslEffect effect, DslExecutionContext context)
    {
        // Move NPC to location
        // effect.Param1 = NPC ID, effect.Param2 = location ID
        if (string.IsNullOrEmpty(effect.Param1) || string.IsNullOrEmpty(effect.Param2))
        {
            context.RecordError("move_npc requires npc_id and location_id");
            return;
        }

        var location = context.GameState.Locations.FirstOrDefault(l => l.Id == effect.Param2);
        if (location is null)
        {
            context.RecordWarning($"Location not found: {effect.Param2}");
            return;
        }

        context.RecordInfo($"Moving NPC {effect.Param1} to {effect.Param2}");
        // Actual NPC movement would be handled by game engine
    }

    private static void ExecuteMessage(DslEffect effect, DslExecutionContext context)
    {
        // Display a message
        // effect.Param1 = message text
        if (string.IsNullOrEmpty(effect.Param1))
        {
            context.RecordError("message requires text");
            return;
        }

        context.RecordInfo($"Message: {effect.Param1}");
        // Actual message display would be handled by game engine
    }

    /// <summary>
    /// Clear the effect cache.
    /// </summary>
    public void ClearCache()
    {
        _compiledEffectCache.Clear();
    }

    /// <summary>
    /// Reset trigger invocation counters (called once per tick).
    /// </summary>
    public void ResetTickCounters()
    {
        lock (_lockObj)
        {
            _triggerInvocationCount.Clear();
        }
    }

    /// <summary>
    /// Get cache statistics.
    /// </summary>
    public (int CacheSize, int CachedEffectStrings) GetCacheStats()
    {
        return (_compiledEffectCache.Count, _compiledEffectCache.Values.Sum(x => x.Count));
    }
}

// <copyright file="DslExecutionContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 execution context for condition/effect evaluation (Slice 086).
/// Encapsulates game state, actor location, and command metadata for safe execution.
/// </summary>
public sealed class DslExecutionContext
{
    /// <summary>
    /// Gets the game state being executed against.
    /// </summary>
    public GameState GameState { get; }

    /// <summary>
    /// Gets the current actor (player or NPC) performing the action.
    /// </summary>
    public string? ActorId { get; set; }

    /// <summary>
    /// Gets the current location context.
    /// </summary>
    public ILocation? CurrentLocation { get; set; }

    /// <summary>
    /// Gets optional command metadata.
    /// </summary>
    public Dictionary<string, object> CommandMetadata { get; } = [];

    /// <summary>
    /// Gets execution diagnostics collected during evaluation.
    /// </summary>
    public List<string> Diagnostics { get; } = [];

    /// <summary>
    /// Gets the current recursion depth for loop detection.
    /// </summary>
    public int RecursionDepth { get; set; }

    /// <summary>
    /// Gets the maximum allowed recursion depth before halting.
    /// </summary>
    public int MaxRecursionDepth { get; set; } = 10;

    /// <summary>
    /// Gets whether execution should stop on first error.
    /// </summary>
    public bool StopOnError { get; set; }

    /// <summary>
    /// Gets whether an error has occurred during execution.
    /// </summary>
    public bool HasError { get; private set; }

    public DslExecutionContext(GameState gameState)
    {
        ArgumentNullException.ThrowIfNull(gameState);
        GameState = gameState;
    }

    /// <summary>
    /// Record an error and potentially stop execution.
    /// </summary>
    public void RecordError(string message)
    {
        HasError = true;
        Diagnostics.Add($"ERROR: {message}");
    }

    /// <summary>
    /// Record a warning message.
    /// </summary>
    public void RecordWarning(string message)
    {
        Diagnostics.Add($"WARNING: {message}");
    }

    /// <summary>
    /// Record an informational message.
    /// </summary>
    public void RecordInfo(string message)
    {
        Diagnostics.Add($"INFO: {message}");
    }

    /// <summary>
    /// Check if recursion limit has been exceeded.
    /// </summary>
    public bool IsRecursionLimitExceeded() => RecursionDepth > MaxRecursionDepth;

    /// <summary>
    /// Clear execution state for next evaluation.
    /// </summary>
    public void Reset()
    {
        RecursionDepth = 0;
        HasError = false;
        Diagnostics.Clear();
    }
}

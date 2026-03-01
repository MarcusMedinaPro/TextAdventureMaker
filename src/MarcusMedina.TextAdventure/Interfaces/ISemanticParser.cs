// <copyright file="ISemanticParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Represents a parsed command intent extracted from natural language input.
/// </summary>
public sealed record ParsedIntent(
    string Verb,
    string? DirectObject,
    string? IndirectObject,
    string? Preposition,
    Direction? Direction,
    Dictionary<string, string> Modifiers,
    float Confidence
);

/// <summary>
/// Context for semantic parsing including available verbs, objects, and NPCs.
/// </summary>
public sealed record ParserContext(
    IGameState State,
    IEnumerable<string> AvailableVerbs,
    IEnumerable<string> VisibleObjects,
    IEnumerable<string> InventoryItems,
    IEnumerable<string> NpcNames
);

/// <summary>
/// Result of disambiguation when multiple objects match the input.
/// </summary>
public sealed record DisambiguationResult(
    bool Resolved,
    string? ResolvedName,
    string? Message,
    List<string>? Options = null
);

/// <summary>
/// Interface for semantic parsing of natural language commands.
/// </summary>
public interface ISemanticParser
{
    /// <summary>
    /// Parses natural language input into a structured intent.
    /// </summary>
    ParsedIntent Parse(string input, ParserContext context);

    /// <summary>
    /// Gets confidence score for a parsed intent.
    /// </summary>
    float GetConfidence(ParsedIntent intent);

    /// <summary>
    /// Gets command suggestions for partial input.
    /// </summary>
    IEnumerable<string> GetSuggestions(string partialInput, ParserContext context);
}

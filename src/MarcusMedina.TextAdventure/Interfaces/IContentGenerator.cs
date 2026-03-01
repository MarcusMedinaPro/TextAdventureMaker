// <copyright file="IContentGenerator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Context for procedural content generation.
/// </summary>
public sealed record GenerationContext(
    IGameState State,
    string Theme,
    DifficultyLevel Difficulty,
    IReadOnlyList<string> UsedIds
);

/// <summary>
/// Interface for procedurally generating game content.
/// </summary>
public interface IContentGenerator<T>
{
    /// <summary>
    /// Generates a single piece of content.
    /// </summary>
    T Generate(GenerationContext context, int? seed = null);

    /// <summary>
    /// Generates multiple pieces of content.
    /// </summary>
    IEnumerable<T> GenerateMultiple(GenerationContext context, int count, int? seed = null);
}

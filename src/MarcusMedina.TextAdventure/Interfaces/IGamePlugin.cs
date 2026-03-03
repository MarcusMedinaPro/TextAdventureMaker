// <copyright file="IGamePlugin.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Optional plugin that extends the game at two phases:
/// Configure (before Build) and OnGameBuilt (after Build).
/// Install via GameBuilder.UsePlugin().
/// </summary>
public interface IGamePlugin
{
    /// <summary>Called before Build() — register parsers, systems, etc.</summary>
    GameBuilder Configure(GameBuilder builder);

    /// <summary>Called after Build() — inject runtime behaviour (NPC AI, etc.).</summary>
    void OnGameBuilt(Game game);
}

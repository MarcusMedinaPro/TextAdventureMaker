// <copyright file="DslNpcDefinition.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 NPC definition models (Slice 077).
/// </summary>

/// <summary>
/// NPC definition for DSL v2.
/// </summary>
public sealed class DslNpcDef
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string State { get; set; } = "alive"; // alive, dormant, dead, defeated
    public int Health { get; set; } = 100;
    public string Archetype { get; set; } = "";
    public int DiesAt { get; set; } = 0; // Health threshold
    public string Movement { get; set; } = "none"; // none, random, patrol:room1,room2
    public string Description { get; set; } = "";
}

/// <summary>
/// NPC placement definition for DSL v2.
/// </summary>
public sealed class DslNpcPlacement
{
    public string LocationId { get; set; } = "";
    public string NpcId { get; set; } = "";
}

/// <summary>
/// NPC dialog root definition for DSL v2.
/// </summary>
public sealed class DslNpcDialog
{
    public string NpcId { get; set; } = "";
    public string Text { get; set; } = "";
}

/// <summary>
/// NPC acceptance rule for DSL v2.
/// </summary>
public sealed class DslNpcAcceptanceRule
{
    public string NpcId { get; set; } = "";
    public string TargetId { get; set; } = ""; // Typically "player"
    public string Condition { get; set; } = ""; // e.g., "counter.good>95"
    public string Level { get; set; } = ""; // e.g., "friend", "hate_you"
    public string Say { get; set; } = ""; // Dialog text for this level
    public int Priority { get; set; } // Lower = checked first
}

/// <summary>
/// NPC acceptance default rule for DSL v2.
/// </summary>
public sealed class DslNpcAcceptanceDefault
{
    public string NpcId { get; set; } = "";
    public string TargetId { get; set; } = "";
    public string Level { get; set; } = "";
    public string Say { get; set; } = "";
}

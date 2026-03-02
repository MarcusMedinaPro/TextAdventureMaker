// <copyright file="DslWorldInteraction.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 world interaction models for doors, exits, and rooms (Slice 076).
/// </summary>

/// <summary>
/// Door configuration for DSL v2.
/// </summary>
public sealed class DslDoorConfig
{
    public string Id { get; set; } = "";
    public string State { get; set; } = "closed"; // open, closed, locked, destroyed
    public List<string> Aliases { get; set; } = [];
    public Dictionary<string, string> Reactions { get; set; } = []; // action -> text
}

/// <summary>
/// Exit configuration for DSL v2.
/// </summary>
public sealed class DslExitConfig
{
    public string FromLocationId { get; set; } = "";
    public string ToLocationId { get; set; } = "";
    public string Direction { get; set; } = "";
    public bool Hidden { get; set; }
    public string DiscoverIf { get; set; } = ""; // condition
    public int Perception { get; set; } = 50; // 1-100
    public string DoorId { get; set; } = "";
    public bool OneWay { get; set; }
}

/// <summary>
/// Room description definition for DSL v2.
/// </summary>
public sealed class DslRoomDescription
{
    public string LocationId { get; set; } = "";
    public string? DefaultDescription { get; set; }
    public string? FirstVisitDescription { get; set; }
}

/// <summary>
/// Conditional room description for DSL v2.
/// </summary>
public sealed class DslRoomDescriptionCondition
{
    public string LocationId { get; set; } = "";
    public string Condition { get; set; } = "";
    public string Text { get; set; } = "";
}

/// <summary>
/// Room variable definition for DSL v2.
/// </summary>
public sealed class DslRoomVariable
{
    public string LocationId { get; set; } = "";
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}

/// <summary>
/// Room transformation rule for DSL v2.
/// </summary>
public sealed class DslRoomTransform
{
    public string SourceLocationId { get; set; } = "";
    public string TargetLocationId { get; set; } = "";
    public string Condition { get; set; } = "";
    public bool Irreversible { get; set; }
}

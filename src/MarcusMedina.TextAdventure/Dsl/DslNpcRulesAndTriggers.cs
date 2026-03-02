// <copyright file="DslNpcRulesAndTriggers.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 NPC rules and triggers models (Slice 078).
/// </summary>

/// <summary>
/// NPC dialog option definition for DSL v2.
/// </summary>
public sealed class DslNpcDialogOption
{
    public string NpcId { get; set; } = "";
    public string FromNodeId { get; set; } = "";
    public string OptionText { get; set; } = "";
    public string ToNodeId { get; set; } = "";
}

/// <summary>
/// NPC rule definition for DSL v2.
/// </summary>
public sealed class DslNpcRule
{
    public string NpcId { get; set; } = "";
    public string RuleId { get; set; } = "";
    public string Condition { get; set; } = ""; // e.g., "counter.reputation>50"
    public int Priority { get; set; } // Higher = evaluated first
    public string Say { get; set; } = "";
    public string Then { get; set; } = ""; // Effects to apply
}

/// <summary>
/// NPC trigger definition for DSL v2.
/// </summary>
public sealed class DslNpcTrigger
{
    public string NpcId { get; set; } = "";
    public string Sense { get; set; } = ""; // see, hear
    public string Target { get; set; } = ""; // what to detect
    public int After { get; set; } = 0; // Delay in ticks before firing
    public string Say { get; set; } = "";
    public bool SayOnce { get; set; }
    public bool Flee { get; set; }
}

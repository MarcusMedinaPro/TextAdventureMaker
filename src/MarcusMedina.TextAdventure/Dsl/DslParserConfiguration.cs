// <copyright file="DslParserConfiguration.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 parser configuration models (Slice 084).
/// </summary>

/// <summary>
/// Parser option definition for DSL v2.
/// </summary>
public sealed class DslParserOption
{
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}

/// <summary>
/// Command alias definition for DSL v2.
/// </summary>
public sealed class DslCommandAlias
{
    public string Alias { get; set; } = "";
    public string TargetCommand { get; set; } = "";
}

/// <summary>
/// Direction alias definition for DSL v2.
/// </summary>
public sealed class DslDirectionAlias
{
    public string Alias { get; set; } = "";
    public string TargetDirection { get; set; } = ""; // "north", "south", "east", "west", "up", "down"
}

/// <summary>
/// Parser configuration container for DSL v2.
/// </summary>
/// <summary>
/// Custom verb declared via <c>command:</c> DSL keyword.
/// </summary>
public sealed class DslCustomVerb
{
    public string Verb { get; set; } = "";
}

/// <summary>
/// NPC reaction definition from DSL <c>npc_reaction:</c> keyword.
/// </summary>
public sealed class DslNpcReaction
{
    public string NpcId { get; set; } = "";
    public string Trigger { get; set; } = "";
    public string Text { get; set; } = "";
    public string? Condition { get; set; }
}

public sealed class DslParserConfiguration
{
    public List<DslParserOption> Options { get; set; } = [];
    public List<DslCommandAlias> CommandAliases { get; set; } = [];
    public List<DslDirectionAlias> DirectionAliases { get; set; } = [];
    public List<DslCustomVerb> CustomVerbs { get; set; } = [];
    public List<DslNpcReaction> NpcReactions { get; set; } = [];

    /// <summary>
    /// Validate parser configuration.
    /// </summary>
    public List<string> Validate()
    {
        var errors = new List<string>();

        // Check for duplicate command aliases
        var commandAliasNames = CommandAliases.Select(a => a.Alias).ToList();
        if (commandAliasNames.Count != commandAliasNames.Distinct().Count())
            errors.Add("Duplicate command aliases found");

        // Check for duplicate direction aliases
        var directionAliasNames = DirectionAliases.Select(a => a.Alias).ToList();
        if (directionAliasNames.Count != directionAliasNames.Distinct().Count())
            errors.Add("Duplicate direction aliases found");

        // Validate direction targets
        var validDirections = new[] { "north", "south", "east", "west", "up", "down", "n", "s", "e", "w", "u", "d" };
        foreach (var alias in DirectionAliases)
        {
            if (!validDirections.Contains(alias.TargetDirection.ToLowerInvariant()))
                errors.Add($"Invalid direction alias target: '{alias.TargetDirection}'");
        }

        return errors;
    }
}

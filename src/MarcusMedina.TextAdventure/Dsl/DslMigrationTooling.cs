// <copyright file="DslMigrationTooling.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v1 to v2 migration tooling (Slice 089).
/// Automates conversion from v1-adventures to v2 world/state structure.
/// </summary>

/// <summary>
/// Migration report tracking converted sections and issues.
/// </summary>
public sealed class DslMigrationReport
{
    public string SourceFile { get; set; } = "";
    public DateTime MigratedAt { get; set; }
    public int TotalSections { get; set; }
    public int ConvertedSections { get; set; }
    public List<string> ConvertedTypes { get; set; } = [];
    public List<DslMigrationWarning> Warnings { get; set; } = [];
    public List<DslMigrationTodo> Todos { get; set; } = [];
    public bool HasRegressionRisks { get; set; }
    public string RegressionSummary { get; set; } = "";

    public int GetConversionRate() =>
        TotalSections > 0 ? (ConvertedSections * 100) / TotalSections : 0;
}

/// <summary>
/// Migration warning for unresolved semantics.
/// </summary>
public sealed class DslMigrationWarning
{
    public string Category { get; set; } = ""; // semantic, performance, structure
    public string Message { get; set; } = "";
    public string? Suggestion { get; set; }
    public int LineNumber { get; set; }
}

/// <summary>
/// Manual TODO marker in migrated content.
/// </summary>
public sealed class DslMigrationTodo
{
    public string Category { get; set; } = ""; // rich-option, reference, custom-logic
    public string Message { get; set; } = "";
    public string? SuggestedApproach { get; set; }
    public int LineNumber { get; set; }
}

/// <summary>
/// Mapping rule for v1 -> v2 conversion.
/// </summary>
public sealed class DslMigrationMappingRule
{
    public string V1Keyword { get; set; } = "";
    public string V2Keyword { get; set; } = "";
    public Func<string, string>? Transform { get; set; }
    public bool RequiresManualReview { get; set; }
}

/// <summary>
/// Migration statistics and analysis.
/// </summary>
public sealed class DslMigrationStats
{
    public int LocationsConverted { get; set; }
    public int ItemsConverted { get; set; }
    public int DoorsConverted { get; set; }
    public int ExitsConverted { get; set; }
    public int MetadataFieldsPreserved { get; set; }
    public int RichOptionsDetected { get; set; }
    public int CustomLogicDetected { get; set; }
}

/// <summary>
/// V1 to V2 migration engine.
/// </summary>
public sealed class DslMigrationEngine
{
    private readonly Dictionary<string, DslMigrationMappingRule> _mappingRules = new();

    public DslMigrationEngine()
    {
        SetupDefaultMappings();
    }

    private void SetupDefaultMappings()
    {
        // Location mapping: v1 location -> v2 location
        _mappingRules["location"] = new DslMigrationMappingRule
        {
            V1Keyword = "location",
            V2Keyword = "location",
            RequiresManualReview = false
        };

        // Item mapping: v1 item -> v2 define item + place item
        _mappingRules["item"] = new DslMigrationMappingRule
        {
            V1Keyword = "item",
            V2Keyword = "define item",
            RequiresManualReview = false
        };

        // Key mapping: v1 key -> v2 define key
        _mappingRules["key"] = new DslMigrationMappingRule
        {
            V1Keyword = "key",
            V2Keyword = "define key",
            RequiresManualReview = false
        };

        // Door mapping: v1 door -> v2 door_config
        _mappingRules["door"] = new DslMigrationMappingRule
        {
            V1Keyword = "door",
            V2Keyword = "door_config",
            RequiresManualReview = false
        };

        // Exit mapping: v1 exit -> v2 exit_config
        _mappingRules["exit"] = new DslMigrationMappingRule
        {
            V1Keyword = "exit",
            V2Keyword = "exit_config",
            RequiresManualReview = false
        };
    }

    /// <summary>
    /// Migrate v1 content to v2 format.
    /// </summary>
    public (string WorldDsl, string StateDsl, DslMigrationReport Report) Migrate(string v1Content, string sourceFileName = "")
    {
        ArgumentNullException.ThrowIfNull(v1Content);

        var report = new DslMigrationReport
        {
            SourceFile = sourceFileName,
            MigratedAt = DateTime.UtcNow
        };

        var stats = new DslMigrationStats();

        var lines = v1Content.Split('\n');
        var worldLines = new List<string>();
        var stateLines = new List<string>();
        var metadata = new Dictionary<string, string>();

        // Phase 1: Extract metadata and classify sections
        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (trimmed.StartsWith("world:"))
            {
                metadata["world"] = trimmed[6..].Trim();
                report.ConvertedTypes.Add("world");
                report.ConvertedSections++;
            }
            else if (trimmed.StartsWith("goal:"))
            {
                metadata["goal"] = trimmed[5..].Trim();
                report.ConvertedTypes.Add("goal");
                report.ConvertedSections++;
            }
            else if (trimmed.StartsWith("start:"))
            {
                metadata["start"] = trimmed[6..].Trim();
                stateLines.Add($"current_location: {trimmed[6..].Trim()}");
                report.ConvertedSections++;
            }
            else if (trimmed.StartsWith("location:"))
            {
                worldLines.Add(line);
                stats.LocationsConverted++;
                report.ConvertedSections++;
            }
            else if (trimmed.StartsWith("item:"))
            {
                ConvertItem(line, worldLines, stats, report);
                report.ConvertedSections++;
            }
            else if (trimmed.StartsWith("key:"))
            {
                worldLines.Add(line.Replace("key:", "define key:"));
                stats.ItemsConverted++;
                report.ConvertedSections++;
            }
            else if (trimmed.StartsWith("door:"))
            {
                ConvertDoor(line, worldLines, stats, report);
                report.ConvertedSections++;
            }
            else if (trimmed.StartsWith("exit:"))
            {
                ConvertExit(line, worldLines, stats, report);
                report.ConvertedSections++;
            }
        }

        report.TotalSections = lines.Length;

        // Phase 2: Add metadata to world file
        var worldDsl = new System.Text.StringBuilder();
        foreach (var (key, value) in metadata)
        {
            worldDsl.AppendLine($"{key}: {value}");
        }
        worldDsl.AppendLine();
        worldDsl.AppendLine(string.Join("\n", worldLines));

        // Phase 3: Build state file
        var stateDsl = new System.Text.StringBuilder();
        stateDsl.AppendLine(string.Join("\n", stateLines));

        // Phase 4: Add TODOs for complex items
        if (stats.CustomLogicDetected > 0)
        {
            report.HasRegressionRisks = true;
            report.RegressionSummary = $"{stats.CustomLogicDetected} items with custom logic detected";
        }

        return (worldDsl.ToString(), stateDsl.ToString(), report);
    }

    private void ConvertItem(string v1Line, List<string> worldLines, DslMigrationStats stats, DslMigrationReport report)
    {
        // Convert v1 item line to v2 define item + place item
        var converted = v1Line.Replace("item:", "define item:");
        worldLines.Add(converted);
        stats.ItemsConverted++;
    }

    private void ConvertDoor(string v1Line, List<string> worldLines, DslMigrationStats stats, DslMigrationReport report)
    {
        // Convert v1 door to v2 door_config
        var converted = v1Line.Replace("door:", "door_config:");
        worldLines.Add(converted);
        stats.DoorsConverted++;
    }

    private void ConvertExit(string v1Line, List<string> worldLines, DslMigrationStats stats, DslMigrationReport report)
    {
        // Convert v1 exit to v2 exit_config
        var converted = v1Line.Replace("exit:", "exit_config:");
        worldLines.Add(converted);
        stats.ExitsConverted++;
    }

    /// <summary>
    /// Validate migration compatibility.
    /// </summary>
    public DslMigrationReport ValidateMigration(string v1Content, string migratedV2Content)
    {
        var report = new DslMigrationReport
        {
            MigratedAt = DateTime.UtcNow
        };

        // Parse both versions and compare core structure
        // This would require parser integration

        return report;
    }

    /// <summary>
    /// Generate migration script for idempotent operations.
    /// </summary>
    public List<string> GenerateMigrationScript(string v1SourcePath, string v2WorldPath, string v2StatePath)
    {
        var script = new List<string>
        {
            "#!/bin/bash",
            "# Generated migration script",
            $"# Source: {v1SourcePath}",
            "",
            "# Create backup",
            $"cp {v1SourcePath} {v1SourcePath}.backup",
            "",
            "# Migrate to v2",
            $"dslhelper migrate --in {v1SourcePath} --out-world {v2WorldPath} --out-state {v2StatePath}",
            "",
            "# Validate migration",
            "dslhelper validate " + v2WorldPath,
            "dslhelper validate " + v2StatePath,
            "",
            "echo 'Migration complete. Review warnings above.'"
        };

        return script;
    }
}

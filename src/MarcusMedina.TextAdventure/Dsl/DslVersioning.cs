// <copyright file="DslVersioning.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 release versioning and deprecation policy (Slice 092).
/// </summary>

/// <summary>
/// Feature flag lifecycle stage.
/// </summary>
public enum FeatureFlagStage
{
    Experimental, // Off by default, may break
    Beta,         // Opt-in, mostly stable
    Stable,       // On by default, fully supported
    Deprecated,   // Scheduled for removal
    Legacy        // Removed, not supported
}

/// <summary>
/// DSL version information.
/// </summary>
public sealed class DslVersion
{
    public int Major { get; set; }
    public int Minor { get; set; }
    public int Patch { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Status { get; set; } = "stable"; // stable, beta, rc, ga
    public string? Notes { get; set; }

    public override string ToString() => $"{Major}.{Minor}.{Patch}";

    /// <summary>
    /// Check if this version is compatible with target version.
    /// </summary>
    public bool IsCompatibleWith(DslVersion target) =>
        Major == target.Major; // Major version determines compatibility
}

/// <summary>
/// Engine to DSL version compatibility.
/// </summary>
public sealed class CompatibilityMatrix
{
    private readonly Dictionary<string, List<DslVersion>> _engineToDslVersions = new();

    public CompatibilityMatrix()
    {
        // Define compatibility
        _engineToDslVersions["1.0.0"] = new List<DslVersion>
        {
            new DslVersion { Major = 1, Minor = 0, Patch = 0, Status = "stable" }
        };

        _engineToDslVersions["2.0.0"] = new List<DslVersion>
        {
            new DslVersion { Major = 2, Minor = 0, Patch = 0, Status = "ga" }
        };
    }

    /// <summary>
    /// Check if engine version supports DSL version.
    /// </summary>
    public bool IsSupported(string engineVersion, DslVersion dslVersion)
    {
        if (!_engineToDslVersions.TryGetValue(engineVersion, out var supported))
            return false;

        return supported.Any(v => v.IsCompatibleWith(dslVersion));
    }

    /// <summary>
    /// Get supported DSL versions for engine.
    /// </summary>
    public List<DslVersion> GetSupportedVersions(string engineVersion) =>
        _engineToDslVersions.TryGetValue(engineVersion, out var versions) ? versions : [];
}

/// <summary>
/// Feature flag with lifecycle tracking.
/// </summary>
public sealed class FeatureFlag
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public FeatureFlagStage Stage { get; set; }
    public DslVersion IntroducedIn { get; set; } = new();
    public DslVersion? DeprecatedIn { get; set; }
    public DslVersion? RemovedIn { get; set; }
    public bool EnabledByDefault { get; set; }
    public string? Replacement { get; set; } // New feature that replaces this
}

/// <summary>
/// Release checklist for DSL versions.
/// </summary>
public sealed class ReleaseChecklist
{
    public DslVersion TargetVersion { get; set; } = new();
    public bool AllFixturesPassing { get; set; }
    public bool DemoCplayhroughVerified { get; set; }
    public bool MigrationToolValidated { get; set; }
    public bool DocumentationComplete { get; set; }
    public bool BreakingChangesDocumented { get; set; }
    public bool DeprecationNoticeSent { get; set; }
    public List<string> RemainingTasks { get; set; } = [];

    /// <summary>
    /// Check if release is ready to proceed.
    /// </summary>
    public bool IsReadyForRelease() =>
        AllFixturesPassing &&
        DemoCplayhroughVerified &&
        MigrationToolValidated &&
        DocumentationComplete &&
        BreakingChangesDocumented &&
        RemainingTasks.Count == 0;
}

/// <summary>
/// Deprecation timeline and migration strategy.
/// </summary>
public sealed class DeprecationPolicy
{
    public string DeprecatedFeature { get; set; } = "";
    public DslVersion DeprecatedIn { get; set; } = new();
    public DslVersion RemovalTargeted { get; set; } = new();
    public int DeprecationWarningPeriodVersions { get; set; } = 2; // Warn for 2 versions
    public string MigrationGuide { get; set; } = "";
    public string? Replacement { get; set; }

    /// <summary>
    /// Get migration duration in approximate time.
    /// </summary>
    public string GetMigrationWindowDescription() =>
        $"Deprecated in v{DeprecatedIn}, removal targeted at v{RemovalTargeted}. " +
        $"Plan for {DeprecationWarningPeriodVersions} release cycles.";
}

/// <summary>
/// DSL v2 version management system.
/// </summary>
public sealed class DslVersionManager
{
    private readonly List<DslVersion> _versions = new();
    private readonly Dictionary<string, FeatureFlag> _features = new();
    private readonly CompatibilityMatrix _compatibility = new();
    private DslVersion _currentVersion = new() { Major = 2, Minor = 0, Patch = 0, Status = "ga" };

    public DslVersionManager()
    {
        RegisterDefaultVersions();
        RegisterFeatureFlags();
    }

    private void RegisterDefaultVersions()
    {
        _versions.Add(new DslVersion
        {
            Major = 2, Minor = 0, Patch = 0,
            ReleaseDate = DateTime.Now,
            Status = "ga",
            Notes = "Initial GA release of DSL v2"
        });
    }

    private void RegisterFeatureFlags()
    {
        // Register features
        _features["rich_item_options"] = new FeatureFlag
        {
            Id = "rich_item_options",
            Name = "Rich Item Options",
            Description = "Extended item properties system",
            Stage = FeatureFlagStage.Stable,
            IntroducedIn = new DslVersion { Major = 2, Minor = 0, Patch = 0 },
            EnabledByDefault = true
        };

        _features["npc_acceptance_rules"] = new FeatureFlag
        {
            Id = "npc_acceptance_rules",
            Name = "NPC Acceptance Rules",
            Description = "Reputation-based NPC interaction ladder",
            Stage = FeatureFlagStage.Stable,
            IntroducedIn = new DslVersion { Major = 2, Minor = 0, Patch = 0 },
            EnabledByDefault = true
        };

        _features["quest_system"] = new FeatureFlag
        {
            Id = "quest_system",
            Name = "Quest System",
            Description = "Complete quest tracking with stages and objectives",
            Stage = FeatureFlagStage.Stable,
            IntroducedIn = new DslVersion { Major = 2, Minor = 0, Patch = 0 },
            EnabledByDefault = true
        };

        _features["dynamic_interpolation"] = new FeatureFlag
        {
            Id = "dynamic_interpolation",
            Name = "Dynamic Interpolation",
            Description = "Template variable substitution in text",
            Stage = FeatureFlagStage.Stable,
            IntroducedIn = new DslVersion { Major = 2, Minor = 0, Patch = 0 },
            EnabledByDefault = true
        };
    }

    /// <summary>
    /// Get current DSL version.
    /// </summary>
    public DslVersion GetCurrentVersion() => _currentVersion;

    /// <summary>
    /// Check if feature is enabled for current version.
    /// </summary>
    public bool IsFeatureEnabled(string featureId)
    {
        if (!_features.TryGetValue(featureId, out var feature))
            return false;

        return feature.Stage switch
        {
            FeatureFlagStage.Experimental => false,
            FeatureFlagStage.Beta => feature.EnabledByDefault,
            FeatureFlagStage.Stable => true,
            FeatureFlagStage.Deprecated => feature.EnabledByDefault,
            FeatureFlagStage.Legacy => false,
            _ => false
        };
    }

    /// <summary>
    /// Get deprecation warning for feature if applicable.
    /// </summary>
    public string? GetDeprecationWarning(string featureId)
    {
        if (!_features.TryGetValue(featureId, out var feature))
            return null;

        if (feature.Stage == FeatureFlagStage.Deprecated)
        {
            var msg = $"Feature '{feature.Name}' is deprecated and scheduled for removal.";
            if (!string.IsNullOrEmpty(feature.Replacement))
                msg += $" Use '{feature.Replacement}' instead.";
            return msg;
        }

        return null;
    }

    /// <summary>
    /// Get all registered features.
    /// </summary>
    public List<FeatureFlag> GetAllFeatures() => _features.Values.ToList();

    /// <summary>
    /// Check engine compatibility with DSL version.
    /// </summary>
    public bool IsEngineCompatible(string engineVersion, string dslVersion)
    {
        var parts = dslVersion.Split('.');
        if (parts.Length != 3) return false;

        var dsl = new DslVersion
        {
            Major = int.Parse(parts[0]),
            Minor = int.Parse(parts[1]),
            Patch = int.Parse(parts[2])
        };

        return _compatibility.IsSupported(engineVersion, dsl);
    }

    /// <summary>
    /// Generate changelog for version range.
    /// </summary>
    public string GenerateChangelog(DslVersion from, DslVersion to)
    {
        var changes = new System.Text.StringBuilder();
        changes.AppendLine($"=== DSL Changes from {from} to {to} ===");
        changes.AppendLine();

        var newFeatures = _features.Values
            .Where(f => f.IntroducedIn.Major >= from.Major && f.IntroducedIn.Minor >= from.Minor)
            .ToList();

        if (newFeatures.Count > 0)
        {
            changes.AppendLine("New Features:");
            foreach (var feature in newFeatures)
                changes.AppendLine($"- {feature.Name}: {feature.Description}");
        }

        return changes.ToString();
    }
}

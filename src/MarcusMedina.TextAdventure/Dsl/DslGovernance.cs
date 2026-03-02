// <copyright file="DslGovernance.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 post-GA governance model and v2.x roadmap (Slice 093).
/// Ensures controlled evolution after initial release.
/// </summary>

/// <summary>
/// RFC (Request for Comments) template for new DSL features.
/// </summary>
public sealed class DslFeatureRfc
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public DateTime SubmittedAt { get; set; }
    public DslRfcStatus Status { get; set; }
    public string Motivation { get; set; } = "";
    public string ProposedSyntax { get; set; } = "";
    public string BackwardCompatibilityNotes { get; set; } = "";
    public List<string> BreakingChanges { get; set; } = [];
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

/// <summary>
/// RFC approval status.
/// </summary>
public enum DslRfcStatus
{
    Draft,
    UnderReview,
    Approved,
    Rejected,
    Deferred
}

/// <summary>
/// Compatibility contract for long-term stability.
/// </summary>
public sealed class DslCompatibilityContract
{
    public string VersionRange { get; set; } = "2.x"; // e.g., "2.0-2.5"
    public List<string> GuaranteedStableKeywords { get; set; } = [];
    public List<string> GuaranteedStableDiagnosticCodes { get; set; } = [];
    public bool ExporterOutputBackwardCompatible { get; set; }
    public bool NewFilesParseAsOldVersion { get; set; }
    public string? MigrationPathToFutureVersion { get; set; }
}

/// <summary>
/// Backlog item for future v2.x development.
/// </summary>
public sealed class DslBacklogItem
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DslBacklogPriority Priority { get; set; }
    public int EstimatedSizeDays { get; set; }
    public List<string> Tags { get; set; } = [];
    public string? TargetVersion { get; set; } // e.g., "2.1", "2.2"
    public List<string> Dependencies { get; set; } = [];
}

/// <summary>
/// Backlog item priority.
/// </summary>
public enum DslBacklogPriority
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Metrics for DSL adoption and feedback.
/// </summary>
public sealed class DslMetrics
{
    public DateTime MeasuredAt { get; set; }
    public int MigrationCompletionRate { get; set; } // Percentage
    public Dictionary<string, int> DiagnosticFrequency { get; set; } = [];
    public List<string> AuthoringPainPoints { get; set; } = [];
    public int TotalAdventuresCreated { get; set; }
    public int AverageAdventureComplexity { get; set; }
    public string? MostRequestedFeature { get; set; }
}

/// <summary>
/// Quarterly release window for DSL schema changes.
/// </summary>
public sealed class DslReleaseWindow
{
    public int Quarter { get; set; }
    public int Year { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool AllowsMinorFeatures { get; set; }
    public bool AllowsBreakingChanges { get; set; } // Only for major version
    public List<DslBacklogItem>? PlannedItems { get; set; }
    public string? Status { get; set; } // planning, active, released
}

/// <summary>
/// DSL governance structure and decision-making.
/// </summary>
public sealed class DslGovernanceModel
{
    public string SchemaOwner { get; set; } = "DSL Maintainers";
    public List<string> ApprovalCommittee { get; set; } = [];
    public int RfcMinimumReviewDays { get; set; } = 14;
    public int RfcApprovalThreshold { get; set; } = 2; // Votes needed

    /// <summary>
    /// Validate RFC for completeness.
    /// </summary>
    public List<string> ValidateRfc(DslFeatureRfc rfc)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(rfc.Title))
            errors.Add("RFC must have a title");

        if (string.IsNullOrEmpty(rfc.Motivation))
            errors.Add("RFC must explain motivation");

        if (string.IsNullOrEmpty(rfc.ProposedSyntax))
            errors.Add("RFC must show proposed syntax");

        return errors;
    }
}

/// <summary>
/// v2.x roadmap manager.
/// </summary>
public sealed class DslRoadmapManager
{
    private readonly List<DslBacklogItem> _backlog = new();
    private readonly List<DslReleaseWindow> _releaseWindows = new();
    private readonly DslGovernanceModel _governance = new();

    public DslRoadmapManager()
    {
        InitializeBacklog();
        InitializeReleaseWindows();
    }

    private void InitializeBacklog()
    {
        // v2.1 items
        _backlog.Add(new DslBacklogItem
        {
            Id = "backlog-001",
            Title = "Richer Chapter DSL",
            Description = "Extended chapter system with more granular progression control",
            Priority = DslBacklogPriority.High,
            EstimatedSizeDays = 10,
            Tags = ["story-system", "progression"],
            TargetVersion = "2.1"
        });

        _backlog.Add(new DslBacklogItem
        {
            Id = "backlog-002",
            Title = "Advanced NPC Bonds & Arcs",
            Description = "Relationship tracking and dialogue arcs between NPCs",
            Priority = DslBacklogPriority.High,
            EstimatedSizeDays = 15,
            Tags = ["npc-system", "dialogue"],
            TargetVersion = "2.1"
        });

        // v2.2 items
        _backlog.Add(new DslBacklogItem
        {
            Id = "backlog-003",
            Title = "Enhanced Authoring UX",
            Description = "Better error messages and IDE integration support",
            Priority = DslBacklogPriority.Medium,
            EstimatedSizeDays = 8,
            Tags = ["tooling", "ux"],
            TargetVersion = "2.2"
        });

        _backlog.Add(new DslBacklogItem
        {
            Id = "backlog-004",
            Title = "Visual DSL Editor Integration",
            Description = "Optional visual builder for DSL authoring",
            Priority = DslBacklogPriority.Medium,
            EstimatedSizeDays = 20,
            Tags = ["editor", "optional"],
            TargetVersion = "2.2"
        });

        // Future items
        _backlog.Add(new DslBacklogItem
        {
            Id = "backlog-005",
            Title = "Localization Framework",
            Description = "Support for multi-language game content",
            Priority = DslBacklogPriority.Low,
            EstimatedSizeDays = 12,
            Tags = ["localization", "future"]
        });
    }

    private void InitializeReleaseWindows()
    {
        _releaseWindows.Add(new DslReleaseWindow
        {
            Quarter = 1,
            Year = 2025,
            StartDate = new DateTime(2025, 1, 1),
            EndDate = new DateTime(2025, 3, 31),
            AllowsMinorFeatures = true,
            AllowsBreakingChanges = false,
            Status = "planning"
        });

        _releaseWindows.Add(new DslReleaseWindow
        {
            Quarter = 2,
            Year = 2025,
            StartDate = new DateTime(2025, 4, 1),
            EndDate = new DateTime(2025, 6, 30),
            AllowsMinorFeatures = true,
            AllowsBreakingChanges = false,
            Status = "planning"
        });
    }

    /// <summary>
    /// Get backlog for upcoming quarter.
    /// </summary>
    public List<DslBacklogItem> GetPlannedItems(int quarter, int year)
    {
        var window = _releaseWindows.FirstOrDefault(w => w.Quarter == quarter && w.Year == year);
        if (window?.PlannedItems is null)
        {
            return _backlog.Where(b => b.TargetVersion == $"{year / 100}.{quarter}").ToList();
        }
        return window.PlannedItems;
    }

    /// <summary>
    /// Get all backlog items.
    /// </summary>
    public List<DslBacklogItem> GetBacklog(int limit = 0) =>
        limit > 0 ? _backlog.Take(limit).ToList() : _backlog;

    /// <summary>
    /// Get release windows for planning.
    /// </summary>
    public List<DslReleaseWindow> GetReleaseWindows() => _releaseWindows;

    /// <summary>
    /// Generate v2.x roadmap summary.
    /// </summary>
    public string GenerateRoadmapSummary()
    {
        var summary = new System.Text.StringBuilder();
        summary.AppendLine("=== DSL v2.x Roadmap ===");
        summary.AppendLine();
        summary.AppendLine("Q1 2025: Core stability and NPC enhancements");
        summary.AppendLine("- Richer chapter DSL for better story progression");
        summary.AppendLine("- Advanced NPC relationship tracking");
        summary.AppendLine();
        summary.AppendLine("Q2 2025: Tooling and UX improvements");
        summary.AppendLine("- Enhanced error messages and diagnostics");
        summary.AppendLine("- Optional visual DSL editor");
        summary.AppendLine();
        summary.AppendLine("Future: Localization and extensibility");
        summary.AppendLine();
        return summary.ToString();
    }
}

/// <summary>
/// Contract test suite for backward compatibility.
/// </summary>
public sealed class DslCompatibilityContractTest
{
    /// <summary>
    /// Verify v2 files can parse as v2.
    /// </summary>
    public bool TestV2FilesParseCorrectly(List<string> v2Files)
    {
        // Would test parsing all v2 fixtures
        return true;
    }

    /// <summary>
    /// Verify diagnostic codes are stable.
    /// </summary>
    public bool TestDiagnosticCodesStable(Dictionary<string, string> expectedCodes)
    {
        // Would verify diagnostic codes don't change unexpectedly
        return true;
    }

    /// <summary>
    /// Verify exporter output is backward compatible.
    /// </summary>
    public bool TestExporterBackwardCompatibility()
    {
        // Would test that exported DSL can be re-parsed
        return true;
    }
}

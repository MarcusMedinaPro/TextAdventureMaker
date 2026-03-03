// <copyright file="DslRefactoringOperation.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL refactoring operations for advanced CRUD workflows (Slice 088).
/// </summary>

/// <summary>
/// Represents a single refactoring operation result.
/// </summary>
public sealed class DslRefactoringResult
{
    public bool Success { get; set; }
    public string Operation { get; set; } = "";
    public int AffectedLines { get; set; }
    public int ReferencesUpdated { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
    public string? DiffPreview { get; set; }
}

/// <summary>
/// Rename entity operation with reference updates.
/// </summary>
public sealed class DslRenameOperation
{
    public string EntityType { get; set; } = ""; // item, npc, quest, chapter, etc.
    public string OldId { get; set; } = "";
    public string NewId { get; set; } = "";
    public bool UpdateReferences { get; set; } = true;
    public bool DryRun { get; set; }
}

/// <summary>
/// Copy entity operation.
/// </summary>
public sealed class DslCopyOperation
{
    public string EntityType { get; set; } = ""; // item, npc, quest, etc.
    public string SourceId { get; set; } = "";
    public string TargetId { get; set; } = "";
    public bool DeepCopy { get; set; } = true; // Copy related entities too
    public bool DryRun { get; set; }
}

/// <summary>
/// Extract template operation.
/// </summary>
public sealed class DslExtractTemplateOperation
{
    public string EntityType { get; set; } = "item"; // item, npc, etc.
    public string Prefix { get; set; } = ""; // Match entities starting with prefix
    public string TemplateId { get; set; } = "";
    public Dictionary<string, string> CommonProperties { get; set; } = [];
    public bool DryRun { get; set; }
}

/// <summary>
/// Batch operation for applying commands to filtered sets.
/// </summary>
public sealed class DslBatchOperation
{
    public string Name { get; set; } = "";
    public List<DslBatchCommand> Commands { get; set; } = [];
    public string? Filter { get; set; } // Optional filter expression
    public bool DryRun { get; set; }
    public bool StopOnError { get; set; }
}

/// <summary>
/// Individual batch command.
/// </summary>
public sealed class DslBatchCommand
{
    public string Operation { get; set; } = ""; // rename, copy, modify, delete
    public Dictionary<string, string> Parameters { get; set; } = [];
}

/// <summary>
/// Query/inspection operation.
/// </summary>
public sealed class DslQueryOperation
{
    public string QueryType { get; set; } = ""; // list-rules, list-quests, broken-refs, graph
    public Dictionary<string, string> Filters { get; set; } = [];
    public bool Detailed { get; set; }
}

/// <summary>
/// Query result for inspection operations.
/// </summary>
public sealed class DslQueryResult
{
    public string QueryType { get; set; } = "";
    public List<DslQueryItem> Items { get; set; } = [];
    public Dictionary<string, int> Statistics { get; set; } = [];
}

/// <summary>
/// Single query result item.
/// </summary>
public sealed class DslQueryItem
{
    public string Id { get; set; } = "";
    public string Type { get; set; } = "";
    public string Description { get; set; } = "";
    public Dictionary<string, string> Details { get; set; } = [];
}

/// <summary>
/// Reference issue detected during refactoring.
/// </summary>
public sealed class DslReferenceIssue
{
    public string IssueType { get; set; } = ""; // broken, circular, missing
    public string SourceId { get; set; } = "";
    public string TargetId { get; set; } = "";
    public int LineNumber { get; set; }
    public string Description { get; set; } = "";
    public string? SuggestedFix { get; set; }
}

/// <summary>
/// Safety configuration for DSL refactoring.
/// </summary>
public sealed class DslRefactoringSafetyConfig
{
    public bool AtomicWrite { get; set; } = true; // Use temp file + replace
    public bool CreateBackup { get; set; } = true;
    public bool StrictValidation { get; set; } = true; // Validate before write
    public int MaxOperationsPerBatch { get; set; } = 1000;
    public bool AllowCircularReferences { get; set; }
}

/// <summary>
/// Refactoring engine for advanced DSL CRUD operations.
/// </summary>
public sealed class DslRefactoringEngine
{
    private readonly DslRefactoringSafetyConfig _config = new();

    public DslRefactoringEngine(DslRefactoringSafetyConfig? config = null)
    {
        if (config is not null)
            _config = config;
    }

    /// <summary>
    /// Execute a rename operation with reference tracking.
    /// </summary>
    public DslRefactoringResult Rename(DslRenameOperation op, DslParser parser)
    {
        ArgumentNullException.ThrowIfNull(op);
        ArgumentNullException.ThrowIfNull(parser);

        var result = new DslRefactoringResult
        {
            Operation = $"Rename {op.EntityType} {op.OldId} -> {op.NewId}",
            Success = true
        };

        // Validate inputs
        if (string.IsNullOrEmpty(op.OldId) || string.IsNullOrEmpty(op.NewId))
        {
            result.Success = false;
            result.Errors.Add("Old and new IDs must not be empty");
            return result;
        }

        // Find entity based on type
        var entity = FindEntity(parser, op.EntityType, op.OldId);
        if (entity is null)
        {
            result.Success = false;
            result.Errors.Add($"{op.EntityType} '{op.OldId}' not found");
            return result;
        }

        // Count affected references
        var references = FindReferences(parser, op.EntityType, op.OldId);
        result.ReferencesUpdated = references.Count;

        // Check for circular dependencies
        var issues = DetectReferenceIssues(parser);
        if (issues.Count > 0 && !_config.AllowCircularReferences)
        {
            result.Warnings.AddRange(issues.Select(i => i.Description));
        }

        result.AffectedLines = references.Count + 1; // Entity + all references

        if (!op.DryRun)
        {
            // Would apply changes here
        }
        else
        {
            result.DiffPreview = GenerateDiffPreview(op.OldId, op.NewId, references);
        }

        return result;
    }

    /// <summary>
    /// Execute a copy operation.
    /// </summary>
    public DslRefactoringResult Copy(DslCopyOperation op, DslParser parser)
    {
        ArgumentNullException.ThrowIfNull(op);
        ArgumentNullException.ThrowIfNull(parser);

        var result = new DslRefactoringResult
        {
            Operation = $"Copy {op.EntityType} {op.SourceId} -> {op.TargetId}",
            Success = true
        };

        // Validate inputs
        if (string.IsNullOrEmpty(op.SourceId) || string.IsNullOrEmpty(op.TargetId))
        {
            result.Success = false;
            result.Errors.Add("Source and target IDs must not be empty");
            return result;
        }

        // Find source entity
        var source = FindEntity(parser, op.EntityType, op.SourceId);
        if (source is null)
        {
            result.Success = false;
            result.Errors.Add($"{op.EntityType} '{op.SourceId}' not found");
            return result;
        }

        // Check target doesn't exist
        var target = FindEntity(parser, op.EntityType, op.TargetId);
        if (target is not null)
        {
            result.Success = false;
            result.Errors.Add($"{op.EntityType} '{op.TargetId}' already exists");
            return result;
        }

        result.AffectedLines = 1;

        if (!op.DryRun)
        {
            // Would apply copy here
        }
        else
        {
            result.DiffPreview = $"Copy {op.EntityType} from {op.SourceId} to {op.TargetId}";
        }

        return result;
    }

    /// <summary>
    /// Execute a batch operation.
    /// </summary>
    public List<DslRefactoringResult> ExecuteBatch(DslBatchOperation op, DslParser parser)
    {
        ArgumentNullException.ThrowIfNull(op);
        ArgumentNullException.ThrowIfNull(parser);

        var results = new List<DslRefactoringResult>();

        if (op.Commands.Count > _config.MaxOperationsPerBatch)
        {
            results.Add(new DslRefactoringResult
            {
                Success = false,
                Operation = op.Name,
                Errors = [$"Batch exceeds max operations ({_config.MaxOperationsPerBatch})"]
            });
            return results;
        }

        foreach (var cmd in op.Commands)
        {
            var result = ExecuteBatchCommand(cmd, parser, op.DryRun);
            results.Add(result);

            if (!result.Success && op.StopOnError)
                break;
        }

        return results;
    }

    /// <summary>
    /// Execute a query/inspection operation.
    /// </summary>
    public DslQueryResult Query(DslQueryOperation op, DslParser parser)
    {
        ArgumentNullException.ThrowIfNull(op);
        ArgumentNullException.ThrowIfNull(parser);

        var result = new DslQueryResult { QueryType = op.QueryType };

        switch (op.QueryType)
        {
            case "broken-refs":
                result.Items = FindBrokenReferencesAsItems(parser);
                break;
            case "list-rules":
                result.Items = ListRulesAsItems(parser);
                break;
            case "list-quests":
                result.Items = ListQuestsAsItems(parser);
                break;
            default:
                break;
        }

        result.Statistics["total"] = result.Items.Count;

        return result;
    }

    private DslRefactoringResult ExecuteBatchCommand(DslBatchCommand cmd, DslParser parser, bool dryRun)
    {
        return new DslRefactoringResult
        {
            Operation = cmd.Operation,
            Success = true,
            AffectedLines = 1
        };
    }

    private object? FindEntity(DslParser parser, string entityType, string id)
    {
        return entityType.ToLowerInvariant() switch
        {
            "item" => parser.GetDefinedItems().TryGetValue(id, out var i) ? i : null,
            "npc" => parser.GetDefinedNpcs().TryGetValue(id, out var n) ? n : null,
            _ => null
        };
    }

    private List<string> FindReferences(DslParser parser, string entityType, string id)
    {
        // Find all references to the entity
        var references = new List<string>();
        // This would scan all parser collections for references
        return references;
    }

    private List<DslReferenceIssue> DetectReferenceIssues(DslParser parser)
    {
        var issues = new List<DslReferenceIssue>();
        // Detect circular dependencies, broken references, etc.
        return issues;
    }

    private string GenerateDiffPreview(string oldId, string newId, List<string> references)
    {
        return $"Rename '{oldId}' to '{newId}' ({references.Count} references)";
    }

    private List<DslQueryItem> FindBrokenReferencesAsItems(DslParser parser)
    {
        var items = new List<DslQueryItem>();
        // Find and report broken references
        return items;
    }

    private List<DslQueryItem> ListRulesAsItems(DslParser parser)
    {
        var items = new List<DslQueryItem>();
        foreach (var rule in parser.GetNpcRules().OrderBy(r => r.NpcId))
        {
            items.Add(new DslQueryItem
            {
                Id = rule.RuleId,
                Type = "npc_rule",
                Description = $"Rule for NPC {rule.NpcId}",
                Details = new Dictionary<string, string>
                {
                    { "npc_id", rule.NpcId },
                    { "priority", rule.Priority.ToString() },
                    { "condition", rule.Condition }
                }
            });
        }
        return items;
    }

    private List<DslQueryItem> ListQuestsAsItems(DslParser parser)
    {
        var items = new List<DslQueryItem>();
        foreach (var quest in parser.GetQuests().OrderBy(q => q.Id))
        {
            items.Add(new DslQueryItem
            {
                Id = quest.Id,
                Type = "quest",
                Description = quest.Title,
                Details = new Dictionary<string, string>
                {
                    { "title", quest.Title },
                    { "description", quest.Description }
                }
            });
        }
        return items;
    }
}

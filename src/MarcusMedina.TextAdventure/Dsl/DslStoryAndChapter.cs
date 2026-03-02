// <copyright file="DslStoryAndChapter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Dsl;

/// <summary>
/// DSL v2 story and chapter models (Slice 083).
/// </summary>

/// <summary>
/// Story branch definition for DSL v2.
/// </summary>
public sealed class DslBranch
{
    public string Id { get; set; } = "";
    public string Condition { get; set; } = ""; // When to activate branch
    public string Effects { get; set; } = ""; // What happens when branch activates
}

/// <summary>
/// Chapter definition for DSL v2.
/// </summary>
public sealed class DslChapter
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Objectives { get; set; } = []; // Objective IDs
    public List<string> NextChapters { get; set; } = []; // Which chapters follow
    public bool IsEnding { get; set; }
    public string UnlockCondition { get; set; } = ""; // Optional condition to unlock chapter
}

/// <summary>
/// Chapter objective definition for DSL v2.
/// </summary>
public sealed class DslChapterObjective
{
    public string ChapterId { get; set; } = "";
    public string ObjectiveId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
}

/// <summary>
/// Chapter ending definition for DSL v2.
/// </summary>
public sealed class DslChapterEnding
{
    public string ChapterId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
}

/// <summary>
/// Story and chapter system for DSL v2.
/// </summary>
public sealed class DslStorySystem
{
    private readonly Dictionary<string, DslBranch> _branches = [];
    private readonly Dictionary<string, DslChapter> _chapters = [];
    private readonly Dictionary<string, DslChapterObjective> _objectives = [];
    private readonly Dictionary<string, DslChapterEnding> _endings = [];

    public void AddBranch(DslBranch branch) => _branches[branch.Id] = branch;
    public DslBranch? GetBranch(string id) => _branches.TryGetValue(id, out var b) ? b : null;

    public void AddChapter(DslChapter chapter) => _chapters[chapter.Id] = chapter;
    public DslChapter? GetChapter(string id) => _chapters.TryGetValue(id, out var c) ? c : null;

    public void AddObjective(DslChapterObjective obj) => _objectives[$"{obj.ChapterId}:{obj.ObjectiveId}"] = obj;
    public IEnumerable<DslChapterObjective> GetChapterObjectives(string chapterId) =>
        _objectives.Values.Where(o => o.ChapterId == chapterId);

    public void AddEnding(DslChapterEnding ending) => _endings[$"{ending.ChapterId}:ending"] = ending;
    public DslChapterEnding? GetChapterEnding(string chapterId) =>
        _endings.Values.FirstOrDefault(e => e.ChapterId == chapterId);

    /// <summary>
    /// Validate story structure.
    /// </summary>
    public List<string> Validate()
    {
        var errors = new List<string>();

        // Check for duplicate branch IDs
        var branchIds = _branches.Keys.ToList();
        if (branchIds.Count != branchIds.Distinct().Count())
            errors.Add("Duplicate branch IDs found");

        // Check for duplicate chapter IDs
        var chapterIds = _chapters.Keys.ToList();
        if (chapterIds.Count != chapterIds.Distinct().Count())
            errors.Add("Duplicate chapter IDs found");

        // Validate chapter references
        foreach (var chapter in _chapters.Values)
        {
            foreach (var nextId in chapter.NextChapters)
            {
                if (!_chapters.ContainsKey(nextId))
                    errors.Add($"Chapter '{chapter.Id}' references non-existent chapter '{nextId}'");
            }

            // Validate objectives exist
            foreach (var objId in chapter.Objectives)
            {
                if (!_objectives.Values.Any(o => o.ChapterId == chapter.Id && o.ObjectiveId == objId))
                    errors.Add($"Chapter '{chapter.Id}' references non-existent objective '{objId}'");
            }
        }

        return errors;
    }
}

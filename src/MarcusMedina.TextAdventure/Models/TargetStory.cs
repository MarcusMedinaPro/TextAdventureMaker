// <copyright file="TargetStory.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

public sealed class TargetStory(string id)
{
    private readonly List<TargetStoryRequirement> _requirements = [];

    public string Id { get; } = id ?? "";
    public IReadOnlyList<TargetStoryRequirement> Requirements => _requirements;

    public TargetStory AddRequirement(string key, int? min = null)
    {
        _requirements.Add(new TargetStoryRequirement(key, min));
        return this;
    }
}

public sealed record TargetStoryRequirement(string Key, int? Min = null);

public sealed class TargetStoryBuilder(TargetStory story)
{
    public TargetStoryBuilder Requires(string key, int min = 0)
    {
        story.AddRequirement(key, min);
        return this;
    }
}

public sealed record TargetStoryResult(string Id, bool Achievable, string Details);

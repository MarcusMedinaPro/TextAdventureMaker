// <copyright file="HeroJourneyStage.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Models;

public sealed class HeroJourneyStage(JourneyStage stage)
{
    public JourneyStage Stage { get; } = stage;
    public string? LocationId { get; private set; }
    public string? NpcId { get; private set; }
    public string? TargetId { get; private set; }
    public bool PointOfNoReturn { get; private set; }
    public IReadOnlyList<string> Notes => _notes;
    public IReadOnlyDictionary<string, string> Data => _data;

    private readonly List<string> _notes = [];
    private readonly Dictionary<string, string> _data = new(StringComparer.OrdinalIgnoreCase);

    public HeroJourneyStage SetLocation(string locationId)
    {
        LocationId = locationId ?? "";
        return this;
    }

    public HeroJourneyStage SetNpc(string npcId)
    {
        NpcId = npcId ?? "";
        return this;
    }

    public HeroJourneyStage SetTarget(string targetId)
    {
        TargetId = targetId ?? "";
        return this;
    }

    public HeroJourneyStage AddNote(string note)
    {
        if (!string.IsNullOrWhiteSpace(note))
        {
            _notes.Add(note);
        }

        return this;
    }

    public HeroJourneyStage SetData(string key, string value)
    {
        if (!string.IsNullOrWhiteSpace(key))
        {
            _data[key] = value ?? "";
        }

        return this;
    }

    public HeroJourneyStage MarkPointOfNoReturn()
    {
        PointOfNoReturn = true;
        return this;
    }
}

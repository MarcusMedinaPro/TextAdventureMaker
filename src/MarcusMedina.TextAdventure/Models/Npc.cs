// <copyright file="Npc.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

public class Npc : INpc
{
    private string _description = "";
    private readonly Dictionary<string, string> _properties = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<DialogRule> _dialogRules = [];

    public string Id { get; }
    public string Name { get; }
    public IDictionary<string, string> Properties => _properties;
    public NpcState State { get; private set; }
    public INpcMovement Movement { get; private set; } = new NoNpcMovement();
    public IDialogNode? DialogRoot { get; private set; }
    public IStats Stats { get; private set; }
    public NpcMemory Memory { get; } = new();
    public IReadOnlyList<DialogRule> DialogRules => _dialogRules;
    public bool IsAlive => State != NpcState.Dead && Stats.Health > 0;

    public Npc(string id, string name, NpcState state = NpcState.Friendly, IStats? stats = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = id;
        Name = name;
        State = state;
        Stats = stats ?? new Stats(20);
    }

    public string GetDescription() => _description;

    public INpc Description(string text)
    {
        _description = text ?? "";
        return this;
    }

    public INpc SetState(NpcState state)
    {
        State = state;
        return this;
    }

    public INpc SetMovement(INpcMovement movement)
    {
        ArgumentNullException.ThrowIfNull(movement);
        Movement = movement;
        return this;
    }

    public ILocation? GetNextLocation(ILocation currentLocation, IGameState state)
    {
        ArgumentNullException.ThrowIfNull(currentLocation);
        ArgumentNullException.ThrowIfNull(state);
        return Movement.GetNextLocation(currentLocation, state);
    }

    public INpc Dialog(string text)
    {
        DialogRoot = new DialogNode(text);
        return this;
    }

    public INpc SetDialog(IDialogNode? dialog)
    {
        DialogRoot = dialog;
        return this;
    }

    public INpc SetStats(IStats stats)
    {
        ArgumentNullException.ThrowIfNull(stats);
        Stats = stats;
        return this;
    }

    public DialogRule AddDialogRule(string id)
    {
        var rule = new DialogRule(id);
        _dialogRules.Add(rule);
        return rule;
    }

    public string? GetRuleBasedDialog(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (_dialogRules.Count == 0)
            return null;

        var context = new DialogContext(state, this, Memory);
        var matching = _dialogRules
            .Where(rule => rule.Matches(context) && rule.GetText(context) != null)
            .ToList();

        if (matching.Count == 0)
            return null;

        var selected = matching
            .OrderByDescending(rule => rule.CriteriaCount)
            .ThenByDescending(rule => rule.PriorityValue)
            .First();

        var text = selected.GetText(context);
        if (string.IsNullOrWhiteSpace(text))
            return null;

        selected.Apply(context);
        Memory.MarkMet();
        return text;
    }
}

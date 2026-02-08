// <copyright file="Npc.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Npc : INpc
{
    private string _description = "";
    private readonly Dictionary<string, string> _properties = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<DialogRule> _dialogRules = [];
    private readonly List<NpcTrigger> _triggers = [];
    private readonly Dictionary<string, ICharacterArc> _arcs = new(StringComparer.OrdinalIgnoreCase);

    public string Id { get; }
    public string Name { get; }
    public IDictionary<string, string> Properties => _properties;
    public NpcState State { get; private set; }
    public INpcMovement Movement { get; private set; } = new NoNpcMovement();
    public IDialogNode? DialogRoot { get; private set; }
    public IStats Stats { get; private set; }
    public NpcMemory Memory { get; } = new();
    public IReadOnlyList<DialogRule> DialogRules => _dialogRules;
    public IReadOnlyList<NpcTrigger> Triggers => _triggers;
    public IReadOnlyDictionary<string, ICharacterArc> Arcs => _arcs;
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

    public string GetDescription()
    {
        return _description;
    }

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
        DialogRule rule = new(id);
        _dialogRules.Add(rule);
        return rule;
    }

    public NpcTrigger OnSee(string target)
    {
        NpcTrigger trigger = new(NpcSense.See, target);
        _triggers.Add(trigger);
        return trigger;
    }

    public NpcTrigger OnHear(string target)
    {
        NpcTrigger trigger = new(NpcSense.Hear, target);
        _triggers.Add(trigger);
        return trigger;
    }

    public ICharacterArc DefineArc(string id)
    {
        CharacterArc arc = new(id);
        _arcs[id] = arc;
        return arc;
    }

    public string? GetRuleBasedDialog(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (_dialogRules.Count == 0)
        {
            return null;
        }

        DialogContext context = new(state, this, Memory);
        List<DialogRule> matching = _dialogRules
            .Where(rule => rule.Matches(context) && rule.GetText(context) != null)
            .ToList();

        if (matching.Count == 0)
        {
            return null;
        }

        DialogRule selected = matching
            .OrderByDescending(rule => rule.CriteriaCount)
            .ThenByDescending(rule => rule.PriorityValue)
            .First();

        string? text = selected.GetText(context);
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        selected.Apply(context);
        Memory.MarkMet();
        return text;
    }
}

// <copyright file="Npc.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Npc(string id, string name, NpcState state = NpcState.Friendly, IStats? stats = null) : INpc
{
    private string _description = "";
    private readonly Dictionary<string, string> _properties = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<DialogRule> _dialogRules = [];
    private readonly List<NpcTrigger> _triggers = [];
    private readonly List<NpcReaction> _reactions = [];
    private readonly List<NpcIdleBehavior> _idleBehaviors = [];
    private readonly Dictionary<string, ICharacterArc> _arcs = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, IBond> _bonds = new(StringComparer.OrdinalIgnoreCase);

    public string Id { get; } = id ?? throw new ArgumentException(null, nameof(id));
    public string Name { get; } = name ?? throw new ArgumentException(null, nameof(name));
    public IDictionary<string, string> Properties => _properties;
    public NpcState State { get; private set; } = state;
    public INpcMovement Movement { get; private set; } = new NoNpcMovement();
    public IDialogNode? DialogRoot { get; private set; }
    public IStats Stats { get; private set; } = stats ?? new Stats(20);
    public NpcMemory Memory { get; } = new();
    public NpcPersonality Personality { get; set; } = new();
    public IReadOnlyList<DialogRule> DialogRules => _dialogRules;
    public IReadOnlyList<NpcTrigger> Triggers => _triggers;
    public IReadOnlyList<NpcReaction> Reactions => _reactions;
    public IReadOnlyList<NpcIdleBehavior> IdleBehaviors => _idleBehaviors;
    public IReadOnlyDictionary<string, ICharacterArc> Arcs => _arcs;
    public IReadOnlyDictionary<string, IBond> Bonds => _bonds;
    public CharacterArchetype? Archetype { get; private set; }
    public JourneyStage? FateStage { get; private set; }
    public bool IsAlive => State != NpcState.Dead && Stats.Health > 0;

    public string GetDescription() => _description;

    public bool Matches(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;
        string target = input.Trim();
        if (Name.TextCompare(target) || Id.TextCompare(target))
            return true;
        string[] nameParts = Name.Split([' ', '-', '_'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (nameParts.Any(part => part.TextCompare(target)))
            return true;
        string[] idParts = Id.Split([' ', '-', '_'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return idParts.Any(part => part.TextCompare(target));
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
        Movement = movement ?? throw new ArgumentNullException(nameof(movement));
        return this;
    }

    public ILocation? GetNextLocation(ILocation currentLocation, IGameState state) =>
        Movement.GetNextLocation(
            currentLocation ?? throw new ArgumentNullException(nameof(currentLocation)),
            state ?? throw new ArgumentNullException(nameof(state)));

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

    public NpcTrigger OnSee(string target)
    {
        var trigger = new NpcTrigger(NpcSense.See, target);
        _triggers.Add(trigger);
        return trigger;
    }

    public NpcTrigger OnHear(string target)
    {
        var trigger = new NpcTrigger(NpcSense.Hear, target);
        _triggers.Add(trigger);
        return trigger;
    }

    public ICharacterArc DefineArc(string id)
    {
        var arc = new CharacterArc(id);
        _arcs[id] = arc;
        return arc;
    }

    public IBond CreateBond(string id)
    {
        var bond = new Bond(id);
        _bonds[id] = bond;
        return bond;
    }

    public INpc SetArchetype(CharacterArchetype archetype)
    {
        Archetype = archetype;
        return this;
    }

    public INpc DiesAt(JourneyStage stage)
    {
        FateStage = stage;
        return this;
    }

    public INpc AddIdleBehavior(int interval, params string[] messages)
    {
        _idleBehaviors.Add(new NpcIdleBehavior(interval, messages));
        return this;
    }

    public INpc AddReaction(string trigger, string text, Func<IGameState, bool>? condition = null, bool endGame = false, Action<IGameState>? effect = null)
    {
        _reactions.Add(new NpcReaction(trigger.ToLowerInvariant(), text, condition, endGame, effect));
        return this;
    }

    public NpcReaction? GetReaction(string trigger, IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        string lower = trigger.ToLowerInvariant();
        return _reactions.FirstOrDefault(r =>
            r.Trigger == lower &&
            (r.Condition is null || r.Condition(state)));
    }

    public string? GetRuleBasedDialog(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (_dialogRules.Count == 0)
            return null;

        var context = new DialogContext(state, this, Memory);
        var matching = _dialogRules
            .Where(rule => rule.Matches(context) && rule.GetText(context)  is not null)
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

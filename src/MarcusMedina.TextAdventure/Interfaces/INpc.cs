// <copyright file="INpc.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface INpc : IGameEntity
{
    new string Id { get; }
    new string Name { get; }
    string GetDescription();
    NpcState State { get; }
    bool IsAlive { get; }
    INpcMovement Movement { get; }
    IDialogNode? DialogRoot { get; }
    IStats Stats { get; }
    NpcMemory Memory { get; }
    NpcPersonality Personality { get; }
    IReadOnlyList<DialogRule> DialogRules { get; }
    IReadOnlyList<NpcTrigger> Triggers { get; }
    IReadOnlyList<NpcReaction> Reactions { get; }
    IReadOnlyList<NpcIdleBehavior> IdleBehaviors { get; }
    IReadOnlyDictionary<string, ICharacterArc> Arcs { get; }
    IReadOnlyDictionary<string, IBond> Bonds { get; }
    CharacterArchetype? Archetype { get; }
    JourneyStage? FateStage { get; }

    INpc Description(string text);
    INpc SetState(NpcState state);
    INpc SetMovement(INpcMovement movement);
    ILocation? GetNextLocation(ILocation currentLocation, IGameState state);
    INpc Dialog(string text);
    INpc SetDialog(IDialogNode? dialog);
    INpc SetStats(IStats stats);
    DialogRule AddDialogRule(string id);
    NpcTrigger OnSee(string target);
    NpcTrigger OnHear(string target);
    ICharacterArc DefineArc(string id);
    IBond CreateBond(string id);
    INpc SetArchetype(CharacterArchetype archetype);
    INpc DiesAt(JourneyStage stage);
    string? GetRuleBasedDialog(IGameState state);
    INpc AddReaction(string trigger, string text, Func<IGameState, bool>? condition = null, bool endGame = false, Action<IGameState>? effect = null);
    NpcReaction? GetReaction(string trigger, IGameState state);
    INpc AddIdleBehavior(int interval, params string[] messages);
}

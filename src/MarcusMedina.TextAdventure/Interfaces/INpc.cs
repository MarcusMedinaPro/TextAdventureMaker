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
    IReadOnlyList<DialogRule> DialogRules { get; }
    IReadOnlyList<NpcTrigger> Triggers { get; }
    IReadOnlyDictionary<string, ICharacterArc> Arcs { get; }

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
    string? GetRuleBasedDialog(IGameState state);
}

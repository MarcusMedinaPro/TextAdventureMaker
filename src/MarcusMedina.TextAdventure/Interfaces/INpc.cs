// <copyright file="INpc.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public interface INpc : IGameEntity
{
    IDialogNode? DialogRoot { get; }
    IReadOnlyList<DialogRule> DialogRules { get; }
    new string Id { get; }
    bool IsAlive { get; }
    NpcMemory Memory { get; }
    INpcMovement Movement { get; }
    new string Name { get; }
    NpcState State { get; }

    IStats Stats { get; }

    DialogRule AddDialogRule(string id);

    INpc Description(string text);

    INpc Dialog(string text);

    string GetDescription();

    ILocation? GetNextLocation(ILocation currentLocation, IGameState state);

    string? GetRuleBasedDialog(IGameState state);

    INpc SetDialog(IDialogNode? dialog);

    INpc SetMovement(INpcMovement movement);

    INpc SetState(NpcState state);

    INpc SetStats(IStats stats);
}
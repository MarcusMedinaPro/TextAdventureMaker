// <copyright file="DialogContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class DialogContext
{
    public IGameState State { get; }
    public INpc Npc { get; }
    public NpcMemory NpcMemory { get; }
    public ILocation? Location { get; }
    public bool FirstMeeting { get; }

    public DialogContext(IGameState state, INpc npc, NpcMemory npcMemory, ILocation? location = null)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(npc);
        ArgumentNullException.ThrowIfNull(npcMemory);
        State = state;
        Npc = npc;
        NpcMemory = npcMemory;
        Location = location;
        FirstMeeting = !npcMemory.HasMet;
    }
}

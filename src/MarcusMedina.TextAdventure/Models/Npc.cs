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

    public string Id { get; }
    public string Name { get; }
    public IDictionary<string, string> Properties => _properties;
    public NpcState State { get; private set; }
    public INpcMovement Movement { get; private set; } = new NoNpcMovement();
    public IDialogNode? DialogRoot { get; private set; }
    public IStats Stats { get; private set; }
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
}

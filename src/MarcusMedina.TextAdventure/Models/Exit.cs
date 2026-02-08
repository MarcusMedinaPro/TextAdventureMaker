// <copyright file="Exit.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Exit
{
    public ILocation Target { get; }
    public IDoor? Door { get; }
    public bool IsOneWay { get; }
    public bool IsHidden { get; private set; }
    public bool IsDiscovered { get; private set; } = true;
    public Func<IGameState, bool>? DiscoverCondition { get; private set; }
    public int? PerceptionDifficulty { get; private set; }
    public TimedDoor? TimedDoor { get; private set; }

    public bool IsPassable => Door == null || Door.IsPassable;
    public bool IsVisible => !IsHidden || IsDiscovered;

    public Exit(ILocation target, IDoor? door = null, bool isOneWay = false)
    {
        ArgumentNullException.ThrowIfNull(target);
        Target = target;
        Door = door;
        IsOneWay = isOneWay;
    }

    public Exit MarkHidden(Func<IGameState, bool>? discoverCondition = null)
    {
        IsHidden = true;
        IsDiscovered = false;
        DiscoverCondition = discoverCondition;
        return this;
    }

    public Exit WithPerceptionCheck(int difficulty)
    {
        PerceptionDifficulty = Math.Clamp(difficulty, 1, 100);
        return this;
    }

    public TimedDoor WithTimedDoor(string doorId)
    {
        TimedDoor = new TimedDoor(doorId);
        return TimedDoor;
    }

    public bool TryDiscover(IGameState state)
    {
        if (!IsHidden || IsDiscovered)
        {
            return false;
        }

        if (DiscoverCondition != null && !DiscoverCondition(state))
        {
            return false;
        }

        if (PerceptionDifficulty.HasValue)
        {
            int roll = Random.Shared.Next(1, 101);
            if (roll < PerceptionDifficulty.Value)
            {
                return false;
            }
        }

        if (DiscoverCondition == null || DiscoverCondition(state))
        {
            IsDiscovered = true;
            return true;
        }

        return false;
    }
}

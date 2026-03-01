// <copyright file="EventChainExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Extensions;

public static class EventChainExtensions
{
    public static IEventChain StepWhenInLocation(this IEventChain chain, string locationId, Action<IGameState> action)
    {
        return chain.Step(state => state.IsCurrentRoomId(locationId), action);
    }

    public static IEventChain StepWhenFlag(this IEventChain chain, string flag, bool expected, Action<IGameState> action)
    {
        return chain.Step(state => state.WorldState.GetFlag(flag) == expected, action);
    }

    public static IEventChain StepWhenCounterAtLeast(this IEventChain chain, string counter, int minimum, Action<IGameState> action)
    {
        return chain.Step(state => state.WorldState.GetCounter(counter) >= minimum, action);
    }

    public static IEventChain StepWhenTimePhase(this IEventChain chain, TimePhase phase, Action<IGameState> action)
    {
        return chain.Step(state => state.TimeSystem.CurrentPhase == phase, action);
    }

    public static IEventChain StepAfterTicks(this IEventChain chain, int tick, Action<IGameState> action)
    {
        return chain.Step(state => state.TimeSystem.CurrentTick >= tick, action);
    }
}

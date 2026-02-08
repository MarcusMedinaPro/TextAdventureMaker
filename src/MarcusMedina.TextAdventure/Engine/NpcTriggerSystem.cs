// <copyright file="NpcTriggerSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class NpcTriggerSystem
{
    private int _tick;

    public void Attach(IGameState state, IEventSystem events)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(events);

        events.Subscribe(GameEventType.EnterLocation, e =>
        {
            if (e.Location != null)
            {
                TriggerSense(state, e.Location, NpcSense.See, "player");
            }
        });

        events.Subscribe(GameEventType.CombatStart, _ =>
        {
            TriggerSense(state, state.CurrentLocation, NpcSense.Hear, "combat");
        });
    }

    public void Tick(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        int now = state.TimeSystem.Enabled ? state.TimeSystem.CurrentTick : ++_tick;
        foreach (ILocation location in state.Locations)
        {
            foreach (INpc npc in location.Npcs)
            {
                ProcessScheduledTriggers(state, location, npc, now);
            }
        }
    }

    private void TriggerSense(IGameState state, ILocation location, NpcSense sense, string target)
    {
        int now = state.TimeSystem.Enabled ? state.TimeSystem.CurrentTick : ++_tick;
        foreach (INpc npc in location.Npcs)
        {
            foreach (NpcTrigger trigger in npc.Triggers.Where(t => t.Matches(sense, target)))
            {
                ScheduleOrFire(state, location, npc, trigger, now);
            }
        }
    }

    private static void ProcessScheduledTriggers(IGameState state, ILocation location, INpc npc, int now)
    {
        foreach (NpcTrigger trigger in npc.Triggers.Where(t => t.ScheduledTick.HasValue && !t.HasFired && t.ScheduledTick <= now))
        {
            Fire(state, location, npc, trigger);
        }
    }

    private static void ScheduleOrFire(IGameState state, ILocation location, INpc npc, NpcTrigger trigger, int now)
    {
        if (trigger.SayOnlyOnce && trigger.HasFired)
        {
            return;
        }

        if (trigger.DelayTicks <= 0)
        {
            Fire(state, location, npc, trigger);
            return;
        }

        if (!trigger.ScheduledTick.HasValue)
        {
            trigger.ScheduledTick = now + trigger.DelayTicks;
        }
    }

    private static void Fire(IGameState state, ILocation location, INpc npc, NpcTrigger trigger)
    {
        if (trigger.SayOnlyOnce && trigger.HasFired)
        {
            return;
        }

        trigger.HasFired = true;
        trigger.ScheduledTick = null;
        DialogContext context = new(state, npc, npc.Memory, location);
        trigger.Apply(context);

        if (!string.IsNullOrWhiteSpace(trigger.Message))
        {
            state.Events.Publish(new GameEvent(GameEventType.NpcTriggered, state, location, npc: npc, detail: trigger.Message));
        }

        if (trigger.ShouldFlee)
        {
            _ = npc.SetState(NpcState.Neutral);
            location.RemoveNpc(npc);
        }
    }
}

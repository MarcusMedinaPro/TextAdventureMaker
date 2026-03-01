// <copyright file="ActionTriggerSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class ActionTriggerSystem : IActionTriggerSystem
{
    private readonly Dictionary<string, List<Action<ActionTriggerContext>>> _actionHandlers = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<Action<ActionTriggerContext>>> _npcDeathHandlers = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<Action<ActionTriggerContext>>> _itemPickupHandlers = new(StringComparer.OrdinalIgnoreCase);

    public void OnAction(string actionId, Action<ActionTriggerContext> handler)
    {
        Register(_actionHandlers, actionId, handler);
    }

    public void OnNpcDeath(string npcId, Action<ActionTriggerContext> handler)
    {
        Register(_npcDeathHandlers, npcId, handler);
    }

    public void OnItemPickup(string itemId, Action<ActionTriggerContext> handler)
    {
        Register(_itemPickupHandlers, itemId, handler);
    }

    public void TriggerAction(string actionId, IGameState state, ILocation? location = null)
    {
        Trigger(_actionHandlers, actionId, state, location);
    }

    public void TriggerNpcDeath(string npcId, IGameState state, ILocation? location = null)
    {
        Trigger(_npcDeathHandlers, npcId, state, location);
    }

    public void TriggerItemPickup(string itemId, IGameState state, ILocation? location = null)
    {
        Trigger(_itemPickupHandlers, itemId, state, location);
    }

    private static void Register(Dictionary<string, List<Action<ActionTriggerContext>>> store, string id, Action<ActionTriggerContext> handler)
    {
        if (string.IsNullOrWhiteSpace(id) || handler == null)
        {
            return;
        }

        if (!store.TryGetValue(id, out List<Action<ActionTriggerContext>>? handlers))
        {
            handlers = [];
            store[id] = handlers;
        }

        handlers.Add(handler);
    }

    private static void Trigger(
        Dictionary<string, List<Action<ActionTriggerContext>>> store,
        string id,
        IGameState state,
        ILocation? location)
    {
        if (string.IsNullOrWhiteSpace(id) || state == null)
        {
            return;
        }

        if (!store.TryGetValue(id, out List<Action<ActionTriggerContext>>? handlers))
        {
            return;
        }

        ActionTriggerContext context = new(state, location);
        foreach (Action<ActionTriggerContext> handler in handlers)
        {
            handler(context);
        }
    }
}

// <copyright file="ActionTriggerContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class ActionTriggerContext(IGameState state, ILocation? location = null)
{
    public IGameState State { get; } = state;
    public ILocation? Location { get; } = location;

    public void Message(string text) { _ = text; }
    public void SpawnItem(string itemId, string locationId) { _ = itemId; _ = locationId; }
    public void SpawnItem(string itemId, ILocation location) { _ = itemId; _ = location; }
    public void SpawnNpc(string npcId, string locationId) { _ = npcId; _ = locationId; }
    public void OpenDoor(string doorId) { _ = doorId; }
    public void CollapseDoor(string doorId) { _ = doorId; }
    public void After(int ticks, Action action) { _ = ticks; action?.Invoke(); }
}

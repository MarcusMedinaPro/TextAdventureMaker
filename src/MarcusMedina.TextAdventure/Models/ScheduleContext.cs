// <copyright file="ScheduleContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class ScheduleContext(IGameState state)
{
    public IGameState State { get; } = state;
    public void Message(string text) { _ = text; }
    public void SpawnNpc(string npcId, string locationId) { _ = npcId; _ = locationId; }
    public void SpawnItem(string itemId, string locationId) { _ = itemId; _ = locationId; }
    public void TriggerEvent(string eventId) { _ = eventId; }
    public void OpenDoor(string doorId) { _ = doorId; }
    public void SpawnRandomEncounter(string poolId) { _ = poolId; }
}

// <copyright file="GameEventType.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Enums;

public enum GameEventType
{
    EnterLocation = 0,
    ExitLocation = 1,
    PickupItem = 2,
    DropItem = 3,
    TalkToNpc = 4,
    CombatStart = 5,
    OpenDoor = 6,
    UnlockDoor = 7,
    MoveItem = 8,
    CloseDoor = 9,
    LockDoor = 10,
    DestroyDoor = 11,
    NpcTriggered = 12
}

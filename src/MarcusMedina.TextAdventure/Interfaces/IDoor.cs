// <copyright file="IDoor.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IDoor : IGameEntity
{
    new string Id { get; }
    new string Name { get; }
    string GetDescription();
    DoorState State { get; }
    IKey? RequiredKey { get; }

    bool IsPassable { get; }
    IDoor Description(string text);
    string? GetReaction(DoorAction action);
    IDoor SetReaction(DoorAction action, string text);
    bool Open();
    bool Close();
    bool Lock(IKey key);
    bool Unlock(IKey key);
    bool Destroy();
}

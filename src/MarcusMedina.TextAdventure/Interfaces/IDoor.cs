// <copyright file="IDoor.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;
using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IDoor : IGameEntity
{
    new string Id { get; }
    new string Name { get; }
    string GetDescription();
    DoorState State { get; }
    IKey? RequiredKey { get; }
    IReadOnlyList<string> Aliases { get; }

    event Action<IDoor>? OnOpen;
    event Action<IDoor>? OnClose;
    event Action<IDoor>? OnLock;
    event Action<IDoor>? OnUnlock;
    event Action<IDoor>? OnDestroy;

    bool IsPassable { get; }
    IDoor Description(string text);
    bool Matches(string name);
    IDoor AddAliases(params string[] aliases);
    string? GetReaction(DoorAction action);
    IDoor SetReaction(DoorAction action, string text);
    bool Open();
    bool Close();
    bool Lock(IKey key);
    bool Unlock(IKey key);
    bool Destroy();
}

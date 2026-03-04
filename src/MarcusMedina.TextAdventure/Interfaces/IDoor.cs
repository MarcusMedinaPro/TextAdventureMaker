// <copyright file="IDoor.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

using MarcusMedina.TextAdventure.Enums;

public interface IDoor : IGameEntity, IExaminable, IOpenable
{
    event Action<IDoor>? OnClose;

    event Action<IDoor>? OnDestroy;

    event Action<IDoor>? OnLock;

    event Action<IDoor>? OnOpen;

    event Action<IDoor>? OnUnlock;

    IReadOnlyList<string> Aliases { get; }
    new string Id { get; }
    bool IsPassable { get; }
    new string Name { get; }
    IKey? RequiredKey { get; }

    DoorState State { get; }

    IDoor AddAliases(params string[] aliases);

    IDoor Description(string text);

    bool Destroy();

    string? GetReaction(DoorAction action);

    bool Lock(IKey key);

    IDoor SetReaction(DoorAction action, string text);

    bool Unlock(IKey key);
}
// <copyright file="IEventSystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Interfaces;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

public interface IEventSystem
{
    void Subscribe(GameEventType type, Action<GameEvent> handler);
    void Unsubscribe(GameEventType type, Action<GameEvent> handler);
    void Publish(GameEvent gameEvent);
}

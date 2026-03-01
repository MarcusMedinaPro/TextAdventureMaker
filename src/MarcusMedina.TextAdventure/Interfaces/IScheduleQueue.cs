// <copyright file="IScheduleQueue.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IScheduleQueue
{
    IScheduleQueue At(int tick, Action<ScheduleContext> handler);
    IScheduleQueue Every(int ticks, Action<ScheduleContext> handler);
    IScheduleQueue When(Func<IGameState, bool> condition, Action<ScheduleContext> handler);
}

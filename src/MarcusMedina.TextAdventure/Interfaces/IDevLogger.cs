// <copyright file="IDevLogger.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Engine;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IDevLogger
{
    void LogTurn(GameState state, ICommand command, CommandResult result);
    void Flush();
}

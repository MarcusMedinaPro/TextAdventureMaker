// <copyright file="CommandContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Engine;

public class CommandContext
{
    public GameState State { get; }

    public CommandContext(GameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        State = state;
    }
}

// <copyright file="CommandContext.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;

namespace MarcusMedina.TextAdventure.Commands;

public class CommandContext
{
    public GameState State { get; }

    public CommandContext(GameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        State = state;
    }
}

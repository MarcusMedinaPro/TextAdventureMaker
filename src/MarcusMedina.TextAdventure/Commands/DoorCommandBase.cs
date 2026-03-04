// <copyright file="DoorCommandBase.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Commands;

public abstract class DoorCommandBase(string? target) : ICommand
{
    public string? Target { get; } = target;

    public abstract CommandResult Execute(CommandContext context);

    protected static IDoor? ResolveDoor(ILocation location, string? target)
    {
        if (string.IsNullOrWhiteSpace(target))
            return location.Exits.Values.FirstOrDefault(e => e.Door is not null)?.Door;

        string tok = target.Trim();
        return location.Exits.Values
            .Select(e => e.Door)
            .FirstOrDefault(d => d is not null && d.Matches(tok));
    }
}

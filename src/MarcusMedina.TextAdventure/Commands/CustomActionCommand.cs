// <copyright file="CustomActionCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Commands;

/// <summary>
/// Fired when the player uses a custom verb declared via <c>command:</c> in the DSL.
/// No built-in logic — NPC reactions and author hooks handle the response.
/// </summary>
public sealed class CustomActionCommand(string verb, string? target = null) : ICommand
{
    public string Verb { get; } = verb;
    public string? Target { get; } = target;

    public CommandResult Execute(CommandContext context)
        => CommandResult.Ok(string.Empty); // reactions are appended by NpcReactionResolver
}

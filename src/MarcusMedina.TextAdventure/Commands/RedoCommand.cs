// <copyright file="RedoCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class RedoCommand : ICommand
{
    public string Name => "redo";
    public string[]? Aliases => ["r"];
    public string Description => "Redo the last undone action";

    private readonly GameState _state;

    public RedoCommand(GameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        _state = state;
    }

    public CommandResult Execute(CommandContext context)
    {
        GameMemento? memento = _state.History.Redo();

        if (memento  is null)
        {
            return CommandResult.Fail("Nothing to redo.", GameError.None);
        }

        _state.ApplyMemento(memento);
        return CommandResult.Ok("Redid the action.");
    }
}

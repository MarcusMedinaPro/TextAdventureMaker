// <copyright file="UndoCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class UndoCommand : ICommand
{
    public string Name => "undo";
    public string[]? Aliases => ["u"];
    public string Description => "Undo the last action";

    private readonly GameState _state;

    public UndoCommand(GameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        _state = state;
    }

    public CommandResult Execute(CommandContext context)
    {
        GameMemento? memento = _state.History.Undo();

        if (memento == null)
        {
            return CommandResult.Fail("Nothing to undo.", Enums.GameError.None);
        }

        _state.ApplyMemento(memento);
        return CommandResult.Ok("Undid the last action.");
    }
}

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

    private readonly IGameState? _state;

    public UndoCommand() { }

    public UndoCommand(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        _state = state;
    }

    public CommandResult Execute(CommandContext context)
    {
        IGameState state = _state ?? context.State;
        GameMemento? memento = state.History.Undo();

        if (memento  is null)
        {
            return CommandResult.Fail("Nothing to undo.", Enums.GameError.None);
        }

        state.ApplyMemento(memento);
        return CommandResult.Ok("Undid the last action.");
    }
}

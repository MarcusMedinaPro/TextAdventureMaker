// <copyright file="MementoCaretaker.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

/// <summary>Manages undo/redo history using the Memento pattern.</summary>
public class MementoCaretaker
{
    private readonly Stack<GameMemento> _undoStack = [];
    private readonly Stack<GameMemento> _redoStack = [];
    private readonly int _maxHistorySize;

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    public MementoCaretaker(int maxHistorySize = 50)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(maxHistorySize, 0);
        _maxHistorySize = maxHistorySize;
    }

    /// <summary>Capture a memento for undo/redo history.</summary>
    public void Capture(GameMemento memento)
    {
        ArgumentNullException.ThrowIfNull(memento);
        _undoStack.Push(memento);
        _redoStack.Clear();

        // Enforce maximum history size
        while (_undoStack.Count > _maxHistorySize)
        {
            _ = _undoStack.Pop();
        }
    }

    /// <summary>Undo one step and return the memento to apply.</summary>
    public GameMemento? Undo()
    {
        if (!CanUndo)
        {
            return null;
        }

        GameMemento memento = _undoStack.Pop();
        _redoStack.Push(memento);
        return memento;
    }

    /// <summary>Redo one step and return the memento to apply.</summary>
    public GameMemento? Redo()
    {
        if (!CanRedo)
        {
            return null;
        }

        GameMemento memento = _redoStack.Pop();
        _undoStack.Push(memento);
        return memento;
    }

    /// <summary>Clear all history.</summary>
    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }
}

// <copyright file="DevLogger.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class DevLogger : IDevLogger, IDisposable
{
    private readonly TextWriter _writer;

    public DevLogger(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _writer = new StreamWriter(path, append: true);
    }

    public DevLogger(TextWriter writer)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public void LogTurn(GameState state, ICommand command, CommandResult result)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(result);

        _writer.WriteLine($"[{DateTime.UtcNow:O}] {command.GetType().Name} @ {state.CurrentLocation.Id} -> {result.Success} ({result.Error})");
        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            _writer.WriteLine($"  Message: {result.Message}");
        }
    }

    public void Flush()
    {
        _writer.Flush();
    }

    public void Dispose()
    {
        _writer.Dispose();
    }
}

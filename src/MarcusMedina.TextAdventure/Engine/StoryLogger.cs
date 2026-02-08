// <copyright file="StoryLogger.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class StoryLogger : IStoryLogger, IDisposable
{
    private readonly TextWriter _writer;

    public StoryLogger(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _writer = new StreamWriter(path, append: true);
    }

    public StoryLogger(TextWriter writer)
    {
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public void LogTurn(GameState state, ICommand command, CommandResult result)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(result);

        if (!string.IsNullOrWhiteSpace(result.Message))
        {
            _writer.WriteLine(result.Message);
        }

        foreach (string reaction in result.ReactionsList)
        {
            if (!string.IsNullOrWhiteSpace(reaction))
            {
                _writer.WriteLine($"> {reaction}");
            }
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

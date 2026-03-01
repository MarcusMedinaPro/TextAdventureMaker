// <copyright file="GameExplorer.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Engine;

public sealed class GameExplorer(Game game)
{
    private readonly Game _game = game ?? throw new ArgumentNullException(nameof(game));

    public IReadOnlyList<string> RandomWalk(int maxSteps = 100)
    {
        List<string> log = [];
        ILocation current = _game.State.CurrentLocation;
        for (int i = 0; i < maxSteps; i++)
        {
            log.Add(current.Id);
            if (current.Exits.Count == 0)
            {
                break;
            }

            Exit exit = current.Exits.Values.First();
            current = exit.Target;
        }

        return log;
    }

    public IReadOnlyList<IReadOnlyList<string>> ExploreAllPaths()
    {
        return [];
    }

    public IReadOnlyList<string>? FindWinningPath()
    {
        return null;
    }

    public void Replay(IEnumerable<string> path)
    {
        _ = path;
    }
}

// <copyright file="StoryState.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class StoryState
{
    private readonly List<IStoryBranch> _branches = [];

    public IReadOnlyList<IStoryBranch> Branches => _branches;

    public StoryState AddBranch(IStoryBranch branch)
    {
        ArgumentNullException.ThrowIfNull(branch);
        _branches.Add(branch);
        return this;
    }

    public IEnumerable<IStoryBranch> Check(IGameState state)
    {
        foreach (IStoryBranch branch in _branches)
        {
            if (branch.Evaluate(state))
            {
                yield return branch;
            }
        }
    }
}

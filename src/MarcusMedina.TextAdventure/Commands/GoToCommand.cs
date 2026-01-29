// <copyright file="GoToCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class GoToCommand : ICommand
{
    public string Target { get; }

    public GoToCommand(string target)
    {
        Target = target;
    }

    public CommandResult Execute(CommandContext context)
    {
        IReadOnlyDictionary<Direction, Exit> exits = context.State.CurrentLocation.Exits;
        string? suggestion = null;
        List<KeyValuePair<Direction, Exit>> matches = exits
            .Where(e => e.Value.Door != null && (Target.TextCompare("door") || e.Value.Door.Matches(Target)))
            .ToList();

        if (matches.Count == 0 && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(Target))
        {
            IEnumerable<IDoor> doors = exits.Values.Select(e => e.Door).Where(d => d != null).Cast<IDoor>();
            IDoor? best = FuzzyMatcher.FindBestDoor(doors, Target, context.State.FuzzyMaxDistance);
            if (best != null)
            {
                suggestion = best.Name;
                matches = exits
                    .Where(e => e.Value.Door != null && ReferenceEquals(e.Value.Door, best))
                    .ToList();
            }
        }

        if (matches.Count != 1)
        {
            return CommandResult.Fail(Language.CantGoThatWay, GameError.NoExitInDirection);
        }

        Direction direction = matches[0].Key;
        if (context.State.Move(direction))
        {
            CommandResult result = CommandResult.Ok(Language.GoDirection(direction.ToString().Lower()));
            return suggestion != null ? result.WithSuggestion(suggestion) : result;
        }

        GameError error = context.State.LastMoveErrorCode != GameError.None
            ? context.State.LastMoveErrorCode
            : GameError.NoExitInDirection;
        return CommandResult.Fail(context.State.LastMoveError ?? Language.CantGoThatWay, error);
    }
}

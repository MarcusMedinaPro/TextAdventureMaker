// <copyright file="HintCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public sealed class HintCommand(string? target) : ICommand
{
    public string? Target { get; } = target;

    public CommandResult Execute(CommandContext context)
    {
        if (string.IsNullOrWhiteSpace(Target))
        {
            return CommandResult.Fail(Language.HintWhat, GameError.MissingArgument);
        }

        string token = Target.Trim();
        ILocation? goal = context.State.Locations
            .FirstOrDefault(location => location.Id.TextCompare(token));

        if (goal == null)
        {
            return CommandResult.Fail(Language.UnknownLocation, GameError.LocationNotFound);
        }

        IReadOnlyList<Direction> path = context.State.Pathfinder.FindPath(context.State.CurrentLocation, goal);
        if (path.Count == 0)
        {
            return CommandResult.Fail(Language.NoPathFound, GameError.LocationNotFound);
        }

        string route = string.Join(", ", path.Select(Language.DirectionName));
        return CommandResult.Ok(Language.HintPath(route));
    }
}

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

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
        var exits = context.State.CurrentLocation.Exits;
        var matches = exits
            .Where(e => e.Value.Door != null && (Target.TextCompare("door") || e.Value.Door.Name.TextCompare(Target)))
            .ToList();

        if (matches.Count != 1)
        {
            return CommandResult.Fail(Language.CantGoThatWay, GameError.NoExitInDirection);
        }

        var direction = matches[0].Key;
        if (context.State.Move(direction))
        {
            return CommandResult.Ok(Language.GoDirection(direction.ToString().Lower()));
        }

        var error = context.State.LastMoveErrorCode != GameError.None
            ? context.State.LastMoveErrorCode
            : GameError.NoExitInDirection;
        return CommandResult.Fail(context.State.LastMoveError ?? Language.CantGoThatWay, error);
    }
}

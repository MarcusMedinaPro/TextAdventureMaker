using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class GoCommand : ICommand
{
    public Direction Direction { get; }

    public GoCommand(Direction direction)
    {
        Direction = direction;
    }

    public CommandResult Execute(CommandContext context)
    {
        if (context.State.Move(Direction))
        {
            return CommandResult.Ok(Language.GoDirection(Direction.ToString().Lower()));
        }

        var error = context.State.LastMoveErrorCode != GameError.None
            ? context.State.LastMoveErrorCode
            : GameError.NoExitInDirection;
        return CommandResult.Fail(context.State.LastMoveError ?? Language.CantGoThatWay, error);
    }
}

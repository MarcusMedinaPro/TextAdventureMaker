using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class OpenCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var exitWithDoor = context.State.CurrentLocation.Exits.Values
            .FirstOrDefault(e => e.Door != null);

        if (exitWithDoor?.Door == null)
        {
            return CommandResult.Fail(Language.NoDoorHere, GameError.NoDoorHere);
        }

        if (exitWithDoor.Door.State == DoorState.Open)
        {
            return CommandResult.Fail(Language.DoorAlreadyOpenMessage(exitWithDoor.Door.Name), GameError.DoorAlreadyOpen);
        }

        if (exitWithDoor.Door.Open())
        {
            return CommandResult.Ok(Language.DoorOpened(exitWithDoor.Door.Name));
        }

        var message = exitWithDoor.Door.State == DoorState.Locked
            ? Language.DoorLocked(exitWithDoor.Door.Name)
            : Language.DoorWontBudge(exitWithDoor.Door.Name);

        var error = exitWithDoor.Door.State == DoorState.Locked
            ? GameError.DoorIsLocked
            : GameError.DoorIsClosed;
        return CommandResult.Fail(message, error);
    }
}

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class UnlockCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var exitWithDoor = context.State.CurrentLocation.Exits.Values
            .FirstOrDefault(e => e.Door != null);

        if (exitWithDoor?.Door == null)
        {
            return CommandResult.Fail(Language.NoDoorHere, GameError.NoDoorHere);
        }

        if (exitWithDoor.Door.RequiredKey == null)
        {
            return CommandResult.Fail(Language.NoKeyRequired, GameError.NoKeyRequired);
        }

        var keys = context.State.Inventory.Items.OfType<IKey>().ToList();
        if (keys.Count == 0)
        {
            return CommandResult.Fail(Language.YouNeedAKeyToOpenDoor, GameError.WrongKey);
        }

        foreach (var key in keys)
        {
            if (exitWithDoor.Door.Unlock(key))
            {
                return CommandResult.Ok(Language.DoorUnlocked(exitWithDoor.Door.Name));
            }
        }

        return CommandResult.Fail(Language.ThatKeyDoesNotFit, GameError.WrongKey);
    }
}

using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Commands;

public class UnknownCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        return CommandResult.Fail(Language.UnknownCommand, GameError.UnknownCommand);
    }
}

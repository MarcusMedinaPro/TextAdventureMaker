using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class QuitCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        return CommandResult.Quit(Language.ThanksForPlaying);
    }
}

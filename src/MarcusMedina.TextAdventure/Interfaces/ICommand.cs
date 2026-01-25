using MarcusMedina.TextAdventure.Commands;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ICommand
{
    CommandResult Execute(CommandContext context);
}

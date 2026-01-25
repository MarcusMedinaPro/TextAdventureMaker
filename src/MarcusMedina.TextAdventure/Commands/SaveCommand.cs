using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class SaveCommand : ICommand
{
    public const string DefaultFileName = "savegame.json";
    public string? Target { get; }

    public SaveCommand(string? target = null)
    {
        Target = target;
    }

    public CommandResult Execute(CommandContext context)
    {
        var path = string.IsNullOrWhiteSpace(Target) ? DefaultFileName : Target!;
        try
        {
            var memento = context.State.CreateMemento();
            context.State.SaveSystem.Save(path, memento);
            return CommandResult.Ok(Language.SaveSuccess(path));
        }
        catch (Exception ex)
        {
            return CommandResult.Fail(Language.SaveFailed(path), GameError.InvalidState, ex.Message);
        }
    }
}

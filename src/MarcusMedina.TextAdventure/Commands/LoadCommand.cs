using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class LoadCommand : ICommand
{
    public const string DefaultFileName = "savegame.json";
    public string? Target { get; }

    public LoadCommand(string? target = null)
    {
        Target = target;
    }

    public CommandResult Execute(CommandContext context)
    {
        var path = string.IsNullOrWhiteSpace(Target) ? DefaultFileName : Target!;
        try
        {
            var memento = context.State.SaveSystem.Load(path);
            context.State.ApplyMemento(memento);
            return CommandResult.Ok(Language.LoadSuccess(path));
        }
        catch (Exception ex)
        {
            return CommandResult.Fail(Language.LoadFailed(path), GameError.InvalidState, ex.Message);
        }
    }
}

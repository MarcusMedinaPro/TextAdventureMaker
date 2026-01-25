using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Commands;

public sealed record CommandResult(bool Success, string Message, GameError Error = GameError.None, bool ShouldQuit = false)
{
    public static CommandResult Ok(string message) => new(true, message);
    public static CommandResult Fail(string message, GameError error) => new(false, message, error);
    public static CommandResult Quit(string message) => new(true, message, GameError.None, true);
}

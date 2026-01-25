using MarcusMedina.TextAdventure.Engine;

namespace MarcusMedina.TextAdventure.Commands;

public class CommandContext
{
    public GameState State { get; }

    public CommandContext(GameState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        State = state;
    }
}

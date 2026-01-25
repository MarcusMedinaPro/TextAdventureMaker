using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class StatsCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var stats = context.State.Stats;
        return CommandResult.Ok(Language.HealthStatus(stats.Health, stats.MaxHealth));
    }
}

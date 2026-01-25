using MarcusMedina.TextAdventure.Commands;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface ICombatSystem
{
    CommandResult Attack(IGameState state, INpc target);
    CommandResult Flee(IGameState state, INpc target);
}

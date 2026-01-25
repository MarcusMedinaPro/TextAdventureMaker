using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class AttackCommand : ICommand
{
    public string? Target { get; }

    public AttackCommand(string? target)
    {
        Target = target;
    }

    public CommandResult Execute(CommandContext context)
    {
        if (string.IsNullOrWhiteSpace(Target))
        {
            return CommandResult.Fail(Language.NoTargetToAttack, GameError.MissingArgument);
        }

        var location = context.State.CurrentLocation;
        var npc = location.FindNpc(Target);
        if (npc == null)
        {
            return CommandResult.Fail(Language.NoSuchNpcHere, GameError.TargetNotFound);
        }

        if (context.State.Stats.Health <= 0)
        {
            return CommandResult.Fail(Language.PlayerAlreadyDead, GameError.AlreadyDead);
        }

        if (!npc.IsAlive)
        {
            return CommandResult.Fail(Language.TargetAlreadyDead, GameError.AlreadyDead);
        }

        context.State.Events.Publish(new GameEvent(GameEventType.CombatStart, context.State, location, npc: npc));
        return context.State.CombatSystem.Attack(context.State, npc);
    }
}

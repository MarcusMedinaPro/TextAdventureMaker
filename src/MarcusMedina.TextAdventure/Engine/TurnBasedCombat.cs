// <copyright file="TurnBasedCombat.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Engine;

using System.Text;
using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

public sealed class TurnBasedCombat(int playerDamage = 10, int npcDamage = 4) : ICombatSystem
{
    private readonly int _playerDamage = Math.Max(1, playerDamage);
    private readonly int _npcDamage = Math.Max(1, npcDamage);

    public CommandResult Attack(IGameState state, INpc target)
    {
        if (state.Stats.Health <= 0)
        {
            return CommandResult.Fail(Language.PlayerAlreadyDead, GameError.AlreadyDead);
        }

        if (!target.IsAlive)
        {
            return CommandResult.Fail(Language.TargetAlreadyDead, GameError.AlreadyDead);
        }

        var builder = new StringBuilder();
        _ = builder.Append(Language.AttackTarget(target.Name));
        _ = builder.Append("\n");
        _ = builder.Append(Language.AttackDamage(_playerDamage));

        target.Stats.Damage(_playerDamage);
        if (target.Stats.Health <= 0)
        {
            _ = target.SetState(NpcState.Dead);
            _ = builder.Append("\n");
            _ = builder.Append(Language.TargetDefeated(target.Name));
            return CommandResult.Ok(builder.ToString());
        }

        state.Stats.Damage(_npcDamage);
        _ = builder.Append("\n");
        _ = builder.Append(Language.EnemyAttack(target.Name, _npcDamage));

        if (state.Stats.Health <= 0)
        {
            _ = builder.Append("\n");
            _ = builder.Append(Language.PlayerDefeated);
            return CommandResult.Fail(builder.ToString(), GameError.AlreadyDead);
        }

        return CommandResult.Ok(builder.ToString());
    }

    public CommandResult Flee(IGameState state, INpc? target = null) => CommandResult.Ok(Language.FleeSuccess);
}

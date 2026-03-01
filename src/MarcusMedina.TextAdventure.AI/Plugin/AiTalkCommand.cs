// <copyright file="AiTalkCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.AI.Plugin;

public sealed class AiTalkCommand(string? target, AiFeatureModule module) : ICommand
{
    private readonly string? _target = target;
    private readonly AiFeatureModule _module = module ?? throw new ArgumentNullException(nameof(module));

    public CommandResult Execute(CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (string.IsNullOrWhiteSpace(_target))
            return CommandResult.Fail(Language.NoOneToTalkTo, GameError.MissingArgument);

        INpc? npc = context.State.CurrentLocation.FindNpc(_target);
        if (npc == null)
            return new TalkCommand(_target).Execute(context);

        NpcAiContext npcContext = AiPluginContextFactory.BuildNpcContext(context.State, npc);
        string playerInput = $"Talk to {npc.Name}";
        NpcDialogueResponse? response = _module.NpcDialogue
            .GenerateReplyAsync(npcContext, playerInput)
            .GetAwaiter()
            .GetResult();

        if (response == null || string.IsNullOrWhiteSpace(response.Reply))
            return new TalkCommand(_target).Execute(context);

        if (response.RelationshipDelta != 0)
        {
            int current = context.State.WorldState.GetRelationship(npc.Id);
            int next = Math.Clamp(current + response.RelationshipDelta, -100, 100);
            context.State.WorldState.SetRelationship(npc.Id, next);
        }

        npc.Memory.MarkMet();
        return CommandResult.Ok(response.Reply);
    }
}

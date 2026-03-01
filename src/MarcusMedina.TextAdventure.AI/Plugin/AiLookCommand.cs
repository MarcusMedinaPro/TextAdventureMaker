// <copyright file="AiLookCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.AI.Plugin;

public sealed class AiLookCommand(string? target, AiFeatureModule module) : ICommand
{
    private readonly string? _target = target;
    private readonly AiFeatureModule _module = module ?? throw new ArgumentNullException(nameof(module));

    public CommandResult Execute(CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (!string.IsNullOrWhiteSpace(_target))
        {
            CommandResult targetResult = new LookCommand(_target).Execute(context);
            if (!targetResult.Success)
                return targetResult;

            ILocation location = context.State.CurrentLocation;
            IItem? item = location.FindItem(_target) ?? context.State.Inventory.FindItem(_target);
            INpc? npc = location.FindNpc(_target);
            if (item == null && npc == null)
                return targetResult;

            string entityType;
            string entityId;
            string baselinePrompt;
            if (item != null)
            {
                entityType = "item";
                entityId = item.Id;
                baselinePrompt = item.GetDescription();
            }
            else
            {
                entityType = "npc";
                entityId = npc!.Id;
                baselinePrompt = npc.GetDescription();
            }

            DescriptionRequest request = new(
                EntityType: entityType,
                EntityId: entityId,
                BaselinePrompt: baselinePrompt);

            string? aiDescription = _module.ItemDescriptions.GenerateDescriptionAsync(request).GetAwaiter().GetResult();
            return string.IsNullOrWhiteSpace(aiDescription) ? targetResult : CommandResult.Ok(aiDescription);
        }

        CommandResult baseResult = new LookCommand().Execute(context);
        if (!baseResult.Success)
            return baseResult;

        ILocation currentLocation = context.State.CurrentLocation;
        string baseline = currentLocation.GetDescription();
        DescriptionRequest roomRequest = new(
            EntityType: "room",
            EntityId: currentLocation.Id,
            BaselinePrompt: baseline);

        string? aiRoomDescription = _module.RoomDescriptions.GenerateDescriptionAsync(roomRequest).GetAwaiter().GetResult();
        if (string.IsNullOrWhiteSpace(aiRoomDescription))
            return baseResult;

        return CommandResult.Ok(ReplaceFirstLine(baseResult.Message, aiRoomDescription.Trim()));
    }

    private static string ReplaceFirstLine(string original, string firstLine)
    {
        if (string.IsNullOrWhiteSpace(original))
            return firstLine;

        int lineBreak = original.IndexOf('\n');
        if (lineBreak < 0)
            return firstLine;

        string tail = original[(lineBreak + 1)..];
        return $"{firstLine}\n{tail}";
    }
}

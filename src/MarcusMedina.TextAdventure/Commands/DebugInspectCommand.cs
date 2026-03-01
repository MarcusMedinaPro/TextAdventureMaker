// <copyright file="DebugInspectCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Commands;

public class DebugInspectCommand(GameState state, string[] tokens) : ICommand
{
    public string Name => "inspect";
    public string[]? Aliases => null;
    public string Description => "Inspect an entity (debug only)";

    public CommandResult Execute(CommandContext context)
    {
        if (!state.DebugMode)
            return CommandResult.Fail("Debug mode is not enabled.", GameError.None);

        if (tokens.Length < 2)
            return CommandResult.Fail("Usage: inspect <entity_id>", GameError.None);

        string entityId = string.Join(" ", tokens.Skip(1));
        var sb = new System.Text.StringBuilder();

        // Check location
        if (state.CurrentLocation.Id == entityId)
        {
            string locName = state.CurrentLocation is Models.Location loc ? loc.Name : state.CurrentLocation.Id;
            sb.AppendLine($"Location: {locName}");
            sb.AppendLine($"ID: {state.CurrentLocation.Id}");
            sb.AppendLine($"Items: {state.CurrentLocation.Items.Count}");
            sb.AppendLine($"NPCs: {state.CurrentLocation.Npcs.Count}");
            sb.AppendLine($"Exits: {state.CurrentLocation.Exits.Count}");
            return CommandResult.Ok(sb.ToString());
        }

        // Check items
        var item = state.CurrentLocation.Items.FirstOrDefault(i => i.Id == entityId || i.Matches(entityId));
        if (item != null)
        {
            sb.AppendLine($"Item: {item.Name}");
            sb.AppendLine($"ID: {item.Id}");
            sb.AppendLine($"Takeable: {item.Takeable}");
            return CommandResult.Ok(sb.ToString());
        }

        // Check NPCs
        var npc = state.CurrentLocation.Npcs.FirstOrDefault(n => n.Id == entityId);
        if (npc != null)
        {
            sb.AppendLine($"NPC: {npc.Name}");
            sb.AppendLine($"State: {npc.State}");
            sb.AppendLine($"Health: {npc.Stats.Health}/{npc.Stats.MaxHealth}");
            return CommandResult.Ok(sb.ToString());
        }

        return CommandResult.Fail($"Entity '{entityId}' not found.", GameError.None);
    }
}

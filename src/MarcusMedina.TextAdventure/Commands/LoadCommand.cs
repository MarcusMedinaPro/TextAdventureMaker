// <copyright file="LoadCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

public class LoadCommand : ICommand
{
    public const string DefaultFileName = "savegame.json";
    public string? Target { get; }

    public LoadCommand(string? target = null) => Target = target;

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

// <copyright file="ReadCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using System.Linq;

public class ReadCommand(string target) : ICommand
{
    public string Target { get; } = target;

    public CommandResult Execute(CommandContext context)
    {
        if (string.IsNullOrWhiteSpace(Target))
        {
            return CommandResult.Fail(Language.NothingToRead, GameError.MissingArgument);
        }

        var location = context.State.CurrentLocation;
        var itemInRoom = location.FindItem(Target);
        var itemInInventory = context.State.Inventory.FindItem(Target);
        string? suggestion = null;
        if (itemInRoom is null && itemInInventory is null)
        {
            IEnumerable<IItem> allItems = location.Items.Concat(context.State.Inventory.Items);
            (IItem? bestAny, string? bestSuggestion) = FuzzyItemResolver.Resolve(context.State, allItems, null, Target);
            if (bestAny is not null)
            {
                suggestion = bestSuggestion;
                if (context.State.Inventory.Items.Contains(bestAny))
                    itemInInventory = bestAny;
                else
                    itemInRoom = bestAny;
            }
        }

        var item = itemInInventory ?? itemInRoom;

        if (item  is null)
        {
            return CommandResult.Fail(Language.NothingToRead, GameError.ItemNotFound);
        }

        if (!item.Readable || string.IsNullOrWhiteSpace(item.GetReadText()))
        {
            return CommandResult.Fail(Language.CannotReadThat, GameError.ItemNotUsable);
        }

        if (item.RequiresTakeToRead && itemInInventory  is null)
        {
            return CommandResult.Fail(Language.MustTakeToRead, GameError.ItemNotInInventory);
        }

        if (!item.CanRead(context.State))
        {
            var reaction = item.GetReaction(ItemAction.ReadFailed);
            return reaction  is not null
                ? CommandResult.Fail(Language.TooDarkToRead, GameError.ItemNotUsable, reaction)
                : CommandResult.Fail(Language.TooDarkToRead, GameError.ItemNotUsable);
        }

        var text = item.GetReadText() ?? "";
        var message = item.ReadingCost > 0
            ? Language.ReadingCost(item.ReadingCost, text)
            : text;

        var onRead = item.GetReaction(ItemAction.Read);
        return CommandResultExtensions.OkWithReaction(message, onRead).WithOptionalSuggestion(suggestion);
    }
}

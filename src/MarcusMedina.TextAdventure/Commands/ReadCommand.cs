// <copyright file="ReadCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class ReadCommand : ICommand
{
    public string Target { get; }

    public ReadCommand(string target)
    {
        Target = target;
    }

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
        if (itemInRoom == null && itemInInventory == null &&
            context.State.EnableFuzzyMatching &&
            !FuzzyMatcher.IsLikelyCommandToken(Target))
        {
            var bestRoom = FuzzyMatcher.FindBestItem(location.Items, Target, context.State.FuzzyMaxDistance);
            var bestInventory = FuzzyMatcher.FindBestItem(context.State.Inventory.Items, Target, context.State.FuzzyMaxDistance);
            var best = bestInventory ?? bestRoom;
            if (best != null)
            {
                suggestion = best.Name;
                if (bestInventory != null)
                {
                    itemInInventory = bestInventory;
                }
                else
                {
                    itemInRoom = bestRoom;
                }
            }
        }
        var item = itemInInventory ?? itemInRoom;

        if (item == null)
        {
            return CommandResult.Fail(Language.NothingToRead, GameError.ItemNotFound);
        }

        if (!item.Readable || string.IsNullOrWhiteSpace(item.GetReadText()))
        {
            return CommandResult.Fail(Language.CannotReadThat, GameError.ItemNotUsable);
        }

        if (item.RequiresTakeToRead && itemInInventory == null)
        {
            return CommandResult.Fail(Language.MustTakeToRead, GameError.ItemNotInInventory);
        }

        if (!item.CanRead(context.State))
        {
            var reaction = item.GetReaction(ItemAction.ReadFailed);
            return reaction != null
                ? CommandResult.Fail(Language.TooDarkToRead, GameError.ItemNotUsable, reaction)
                : CommandResult.Fail(Language.TooDarkToRead, GameError.ItemNotUsable);
        }

        var text = item.GetReadText() ?? "";
        var message = item.ReadingCost > 0
            ? Language.ReadingCost(item.ReadingCost, text)
            : text;

        var onRead = item.GetReaction(ItemAction.Read);
        var result = onRead != null
            ? CommandResult.Ok(message, onRead)
            : CommandResult.Ok(message);

        return suggestion != null ? result.WithSuggestion(suggestion) : result;
    }
}

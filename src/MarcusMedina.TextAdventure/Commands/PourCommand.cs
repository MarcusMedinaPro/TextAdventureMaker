// <copyright file="PourCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Commands;

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

public class PourCommand : ICommand
{
    public string FluidName { get; }
    public string ContainerName { get; }

    public PourCommand(string fluidName, string containerName)
    {
        FluidName = fluidName;
        ContainerName = containerName;
    }

    public CommandResult Execute(CommandContext context)
    {
        var inventory = context.State.Inventory;
        var fluidItem = inventory.FindItem(FluidName) as IFluid;
        string? suggestion = null;
        if (fluidItem == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(FluidName))
        {
            if (FuzzyMatcher.FindBestItem(inventory.Items, FluidName, context.State.FuzzyMaxDistance) is IFluid best)
            {
                fluidItem = best;
                suggestion = best.Name;
            }
        }

        if (fluidItem == null)
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotInInventory);
        }

        var containerItem = inventory.FindItem(ContainerName);
        if (containerItem == null && context.State.EnableFuzzyMatching && !FuzzyMatcher.IsLikelyCommandToken(ContainerName))
        {
            var best = FuzzyMatcher.FindBestItem(inventory.Items, ContainerName, context.State.FuzzyMaxDistance);
            if (best != null)
            {
                containerItem = best;
                suggestion ??= best.Name;
            }
        }

        if (containerItem is not IContainer<IFluid> container)
        {
            return CommandResult.Fail(Language.CannotPourThat, GameError.ItemNotUsable);
        }

        if (!container.Add(fluidItem))
        {
            return CommandResult.Fail(Language.InventoryFull, GameError.InventoryFull);
        }

        _ = inventory.Remove((IItem)fluidItem);
        var ok = CommandResult.Ok(Language.PourResult(fluidItem.Name, containerItem.Name));
        return suggestion != null ? ok.WithSuggestion(suggestion) : ok;
    }
}

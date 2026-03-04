// <copyright file="PourCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

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
        IInventory inventory = context.State.Inventory;
        IFluid? fluidItem = inventory.FindItem(FluidName) as IFluid;
        string? suggestion = null;

        if (fluidItem is null)
        {
            (IItem? bestFluidItem, string? fluidSuggestion) = FuzzyItemResolver.Resolve(context.State, inventory.Items, null, FluidName);
            if (bestFluidItem is IFluid bestFluid)
            {
                fluidItem = bestFluid;
                suggestion = fluidSuggestion;
            }
        }

        if (fluidItem  is null)
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotInInventory);
        }

        IItem? containerItem = inventory.FindItem(ContainerName);
        (containerItem, string? containerSuggestion) = FuzzyItemResolver.Resolve(context.State, inventory.Items, containerItem, ContainerName);
        suggestion ??= containerSuggestion;

        if (containerItem is not IContainer<IFluid> container)
        {
            return CommandResult.Fail(Language.CannotPourThat, GameError.ItemNotUsable);
        }

        if (!container.Add(fluidItem))
        {
            return CommandResult.Fail(Language.InventoryFull, GameError.InventoryFull);
        }

        _ = inventory.Remove((IItem)fluidItem);
        string fluidName = Language.EntityName(fluidItem.Id, fluidItem.Name);
        string containerName = Language.EntityName(containerItem);
        return CommandResult.Ok(Language.PourResult(fluidName, containerName)).WithOptionalSuggestion(suggestion);
    }
}

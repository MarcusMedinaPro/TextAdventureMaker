using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;

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
        var inventory = context.State.Inventory;
        var fluidItem = inventory.FindItem(FluidName) as IFluid;
        if (fluidItem == null)
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotInInventory);
        }

        var containerItem = inventory.FindItem(ContainerName);
        if (containerItem is not IContainer<IFluid> container)
        {
            return CommandResult.Fail(Language.CannotPourThat, GameError.ItemNotUsable);
        }

        if (!container.Add(fluidItem))
        {
            return CommandResult.Fail(Language.InventoryFull, GameError.InventoryFull);
        }

        inventory.Remove((IItem)fluidItem);
        return CommandResult.Ok(Language.PourResult(fluidItem.Name, containerItem.Name));
    }
}

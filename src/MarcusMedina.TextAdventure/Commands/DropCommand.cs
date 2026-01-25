using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class DropCommand : ICommand
{
    public string ItemName { get; }

    public DropCommand(string itemName)
    {
        ItemName = itemName;
    }

    public CommandResult Execute(CommandContext context)
    {
        var item = context.State.Inventory.FindItem(ItemName);
        if (item == null)
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotInInventory);
        }

        context.State.Inventory.Remove(item);
        context.State.CurrentLocation.AddItem(item);
        item.Drop();

        return CommandResult.Ok(Language.DropItem(item.Name));
    }
}

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class TakeCommand : ICommand
{
    public string ItemName { get; }

    public TakeCommand(string itemName)
    {
        ItemName = itemName;
    }

    public CommandResult Execute(CommandContext context)
    {
        var item = context.State.CurrentLocation.FindItem(ItemName);
        if (item == null)
        {
            return CommandResult.Fail(Language.NoSuchItemHere, GameError.ItemNotFound);
        }

        if (!item.Takeable)
        {
            return CommandResult.Fail(Language.CannotTakeItem, GameError.ItemNotTakeable);
        }

        var inventory = context.State.Inventory;
        if (!inventory.Add(item))
        {
            if (inventory.LimitType == InventoryLimitType.ByCount)
            {
                return CommandResult.Fail(Language.InventoryFull, GameError.InventoryFull);
            }

            return CommandResult.Fail(Language.TooHeavy, GameError.ItemTooHeavy);
        }

        context.State.CurrentLocation.RemoveItem(item);
        item.Take();

        return CommandResult.Ok(Language.TakeItem(item.Name));
    }
}

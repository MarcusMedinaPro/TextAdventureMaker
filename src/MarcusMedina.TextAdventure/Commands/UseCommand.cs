using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class UseCommand : ICommand
{
    public string ItemName { get; }

    public UseCommand(string itemName)
    {
        ItemName = itemName;
    }

    public CommandResult Execute(CommandContext context)
    {
        var item = context.State.Inventory.FindItem(ItemName);
        if (item == null)
        {
            return CommandResult.Fail(Language.NoSuchItemInventory, GameError.ItemNotFound);
        }

        item.Use();
        return CommandResult.Ok(Language.UseItem(item.Name));
    }
}

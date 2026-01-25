using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class DropAllCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var inventory = context.State.Inventory;
        if (inventory.Count == 0)
        {
            return CommandResult.Fail(Language.NothingToDrop, GameError.ItemNotInInventory);
        }

        var location = context.State.CurrentLocation;
        var items = inventory.Items.ToList();

        foreach (var item in items)
        {
            inventory.Remove(item);
            location.AddItem(item);
            item.Drop();
        }

        var list = items.Select(i => i.Name).CommaJoin();
        return CommandResult.Ok(Language.DropAll(list));
    }
}

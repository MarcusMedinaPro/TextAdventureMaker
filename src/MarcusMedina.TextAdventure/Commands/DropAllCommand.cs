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
        var reactions = new List<string>();

        foreach (var item in items)
        {
            inventory.Remove(item);
            location.AddItem(item);
            item.Drop();
            var reaction = item.GetReaction(ItemAction.Drop);
            if (!string.IsNullOrWhiteSpace(reaction))
            {
                reactions.Add(reaction);
            }
        }

        var list = items.Select(i => i.Name).CommaJoin();
        return reactions.Count > 0
            ? CommandResult.Ok(Language.DropAll(list), reactions.ToArray())
            : CommandResult.Ok(Language.DropAll(list));
    }
}

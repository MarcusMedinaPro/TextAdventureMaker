using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class TakeAllCommand : ICommand
{
    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        var inventory = context.State.Inventory;

        var candidates = location.Items.Where(i => i.Takeable).ToList();
        if (candidates.Count == 0)
        {
            return CommandResult.Fail(Language.NothingToTake, GameError.ItemNotFound);
        }

        var taken = new List<IItem>();
        var skipped = new List<IItem>();

        foreach (var item in candidates)
        {
            if (inventory.Add(item))
            {
                location.RemoveItem(item);
                item.Take();
                taken.Add(item);
            }
            else
            {
                skipped.Add(item);
            }
        }

        if (taken.Count == 0)
        {
            if (inventory.LimitType == InventoryLimitType.ByCount)
            {
                return CommandResult.Fail(Language.InventoryFull, GameError.InventoryFull);
            }

            return CommandResult.Fail(Language.TooHeavy, GameError.ItemTooHeavy);
        }

        var takenList = taken.Select(i => i.Name).CommaJoin();
        var message = Language.TakeAll(takenList);

        if (skipped.Count > 0)
        {
            var skippedList = skipped.Select(i => i.Name).CommaJoin();
            message = $"{message}\n{Language.TakeAllSkipped(skippedList)}";
        }

        return CommandResult.Ok(message);
    }
}

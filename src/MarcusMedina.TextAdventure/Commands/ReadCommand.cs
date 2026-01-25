using MarcusMedina.TextAdventure.Enums;
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
        return onRead != null
            ? CommandResult.Ok(message, onRead)
            : CommandResult.Ok(message);
    }
}

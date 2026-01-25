using System.Text;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class LookCommand : ICommand
{
    public string? Target { get; }

    public LookCommand(string? target = null)
    {
        Target = target;
    }

    public CommandResult Execute(CommandContext context)
    {
        var location = context.State.CurrentLocation;
        if (!string.IsNullOrWhiteSpace(Target))
        {
            return LookAtTarget(context, Target);
        }

        var description = location.GetDescription();
        var exits = location.Exits
            .Select(e => e.Value.Door != null
                ? $"{e.Key} ({e.Value.Door.Name}: {e.Value.Door.State})"
                : e.Key.ToString());

        var builder = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(description))
        {
            builder.Append(description.Trim());
        }

        builder.Append(builder.Length > 0 ? "\n" : string.Empty);
        builder.Append(Language.HealthStatus(context.State.Stats.Health, context.State.Stats.MaxHealth));
        builder.Append("\n");
        var items = location.Items.Select(i => Language.ItemWithWeight(i.Name, i.Weight));
        builder.Append(Language.ItemsHereLabel);
        builder.Append(items.Any() ? items.CommaJoin() : Language.None);
        builder.Append("\n");
        builder.Append(Language.ExitsLabel);
        builder.Append(exits.Any() ? exits.CommaJoin() : Language.None);

        return CommandResult.Ok(builder.ToString());
    }

    private static CommandResult LookAtTarget(CommandContext context, string target)
    {
        var location = context.State.CurrentLocation;
        var item = location.FindItem(target) ?? context.State.Inventory.FindItem(target);
        if (item != null)
        {
            var description = item.GetDescription();
            return CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(item.Name)
                : description);
        }

        var door = location.Exits.Values
            .Select(e => e.Door)
            .FirstOrDefault(d => d != null && (target.TextCompare("door") || d.Name.TextCompare(target)));

        if (door != null)
        {
            var description = door.GetDescription();
            return CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(door.Name)
                : description);
        }

        var key = location.Exits.Values
            .Select(e => e.Door?.RequiredKey)
            .FirstOrDefault(k => k != null && k.Name.TextCompare(target));

        if (key != null)
        {
            var description = key.GetDescription();
            return CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.ItemDescription(key.Name)
                : description);
        }

        if (target.TextCompare(location.Id) || target.TextCompare("here") || target.TextCompare("room"))
        {
            var description = location.GetDescription();
            return CommandResult.Ok(string.IsNullOrWhiteSpace(description)
                ? Language.NothingToLookAt
                : description);
        }

        return CommandResult.Fail(Language.NothingToLookAt, GameError.ItemNotFound);
    }
}

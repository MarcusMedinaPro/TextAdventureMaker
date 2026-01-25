using System.Text;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Commands;

public class TalkCommand : ICommand
{
    public string? Target { get; }

    public TalkCommand(string? target)
    {
        Target = target;
    }

    public CommandResult Execute(CommandContext context)
    {
        if (string.IsNullOrWhiteSpace(Target))
        {
            return CommandResult.Fail(Language.NoOneToTalkTo, GameError.MissingArgument);
        }

        var location = context.State.CurrentLocation;
        var npc = location.FindNpc(Target);
        if (npc == null)
        {
            return CommandResult.Fail(Language.NoSuchNpcHere, GameError.TargetNotFound);
        }

        var dialog = npc.DialogRoot;
        if (dialog == null)
        {
            return CommandResult.Ok(Language.NpcHasNothingToSay);
        }

        var builder = new StringBuilder();
        builder.Append(dialog.Text);

        if (dialog.Options.Count > 0)
        {
            builder.Append("\n");
            builder.Append(Language.DialogOptionsLabel);
            builder.Append("\n");
            for (var i = 0; i < dialog.Options.Count; i++)
            {
                builder.Append(Language.DialogOption(i + 1, dialog.Options[i].Text));
                if (i < dialog.Options.Count - 1)
                {
                    builder.Append("\n");
                }
            }
        }

        return CommandResult.Ok(builder.ToString());
    }
}

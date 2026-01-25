using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public sealed class DialogOption
{
    public string Text { get; }
    public IDialogNode? Next { get; }

    public DialogOption(string text, IDialogNode? next = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        Text = text;
        Next = next;
    }
}

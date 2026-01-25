using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Interfaces;

public interface IDialogNode
{
    string Text { get; }
    IReadOnlyList<DialogOption> Options { get; }
}

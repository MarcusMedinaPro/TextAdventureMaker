using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class ItemCombinationRecipe
{
    public string LeftId { get; }
    public string RightId { get; }
    public Func<IItem> Create { get; }

    public ItemCombinationRecipe(string leftId, string rightId, Func<IItem> create)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(leftId);
        ArgumentException.ThrowIfNullOrWhiteSpace(rightId);
        Create = create ?? throw new ArgumentNullException(nameof(create));
        LeftId = leftId;
        RightId = rightId;
    }

    public bool Matches(IItem a, IItem b)
    {
        return (a.Id == LeftId && b.Id == RightId) || (a.Id == RightId && b.Id == LeftId);
    }
}

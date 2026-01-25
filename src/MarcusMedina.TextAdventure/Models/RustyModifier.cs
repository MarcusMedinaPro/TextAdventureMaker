using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class RustyModifier : ItemDecorator
{
    public RustyModifier(IItem inner) : base(inner)
    {
    }

    public override string Name => $"rusty {Inner.Name}";

    public override string GetDescription()
    {
        var baseDescription = Inner.GetDescription();
        return string.IsNullOrWhiteSpace(baseDescription)
            ? "It looks old and rusty."
            : $"{baseDescription} It's old and rusty.";
    }
}

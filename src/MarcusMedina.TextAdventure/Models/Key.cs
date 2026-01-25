using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Key : Item, IKey
{
    public Key(string id, string name) : base(id, name)
    {
    }

    public new Key SetTakeable(bool takeable)
    {
        base.SetTakeable(takeable);
        return this;
    }

    public new Key SetWeight(float weight)
    {
        base.SetWeight(weight);
        return this;
    }

    public new Key AddAliases(params string[] aliases)
    {
        base.AddAliases(aliases);
        return this;
    }

    public new Key Description(string text)
    {
        base.Description(text);
        return this;
    }
}

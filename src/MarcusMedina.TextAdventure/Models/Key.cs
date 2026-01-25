using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Key : Item, IKey
{
    public Key(string id, string name) : base(id, name)
    {
    }

    public Key(string id, string name, string description) : base(id, name, description)
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

    public new Key SetReaction(ItemAction action, string text)
    {
        base.SetReaction(action, text);
        return this;
    }

    public static implicit operator Key((string id, string name, string description) data) =>
        new(data.id, data.name, data.description);
}

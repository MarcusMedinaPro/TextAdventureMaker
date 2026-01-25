using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class Fluid : IFluid
{
    private string _description = "";

    public string Id { get; }
    public string Name { get; }

    public Fluid(string id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Id = id;
        Name = name;
    }

    public string GetDescription() => _description;

    public IFluid Description(string text)
    {
        _description = text;
        return this;
    }
}

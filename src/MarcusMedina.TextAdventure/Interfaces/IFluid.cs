namespace MarcusMedina.TextAdventure.Interfaces;

public interface IFluid
{
    string Id { get; }
    string Name { get; }
    string GetDescription();
}

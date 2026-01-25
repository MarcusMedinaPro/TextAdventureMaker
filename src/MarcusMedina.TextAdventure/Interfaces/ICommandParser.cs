namespace MarcusMedina.TextAdventure.Interfaces;

public interface ICommandParser
{
    ICommand Parse(string input);
}

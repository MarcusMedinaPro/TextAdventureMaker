using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Dsl;

public interface IDslParser
{
    DslAdventure ParseFile(string path);
}

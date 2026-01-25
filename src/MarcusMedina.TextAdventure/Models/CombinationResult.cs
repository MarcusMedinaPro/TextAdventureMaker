using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class CombinationResult
{
    public bool Success { get; }
    public IReadOnlyList<IItem> Created { get; }

    public CombinationResult(bool success, IReadOnlyList<IItem> created)
    {
        Success = success;
        Created = created;
    }

    public static CombinationResult Fail() => new(false, Array.Empty<IItem>());
    public static CombinationResult Ok(params IItem[] created) => new(true, created);
}

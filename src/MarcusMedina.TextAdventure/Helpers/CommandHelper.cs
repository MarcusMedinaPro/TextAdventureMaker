namespace MarcusMedina.TextAdventure.Helpers;

public static class CommandHelper
{
    /// <summary>
    /// Creates a case-insensitive command set from the provided tokens.
    /// </summary>
    public static HashSet<string> NewCommands(params string[] commands)
    {
        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (commands == null) return set;

        foreach (var command in commands)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                set.Add(command.Trim());
            }
        }

        return set;
    }
}

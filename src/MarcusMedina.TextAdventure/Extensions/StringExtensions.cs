using System.Globalization;

namespace MarcusMedina.TextAdventure.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Formats a localized template using invariant culture for consistent game output.
    /// </summary>
    /// <param name="template">A composite format string like "The {0} is locked."</param>
    /// <param name="args">Values to inject into the template.</param>
    /// <returns>The formatted string.</returns>
    public static string GamePrint(this string template, params object[] args)
    {
        return string.Format(CultureInfo.InvariantCulture, template, args);
    }

    /// <summary>
    /// Lowercases text using invariant culture for consistent parsing behavior.
    /// </summary>
    public static string Lower(this string text)
    {
        return text.ToLower(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Case-insensitive text comparison using ordinal rules.
    /// </summary>
    public static bool TextCompare(this string text, string other)
    {
        return string.Equals(text, other, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Converts a name into a simple id (lowercase, trimmed, spaces to underscores).
    /// </summary>
    public static string ToId(this string text)
    {
        return text.Trim().Lower().Replace(' ', '_');
    }
}

// <copyright file="UserProfile.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Models;

using System.Text.Json;

/// <summary>
/// Represents the player's persistent profile across all games.
/// Used for personalization and NPC romance customization.
/// </summary>
public class UserProfile
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Player's name.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Player's gender identity.
    /// </summary>
    public Gender Gender { get; set; } = Gender.Unspecified;

    /// <summary>
    /// Preferred gender for romantic NPCs. Used to customize romance sequences.
    /// </summary>
    public Gender RomancePreference { get; set; } = Gender.Unspecified;

    /// <summary>
    /// Player's hair color for character descriptions.
    /// </summary>
    public string HairColor { get; set; } = "";

    /// <summary>
    /// Player's eye color for character descriptions.
    /// </summary>
    public string EyeColor { get; set; } = "";

    /// <summary>
    /// Player's age.
    /// </summary>
    public int? Age { get; set; }

    /// <summary>
    /// Player's birthdate for birthday events.
    /// </summary>
    public DateOnly? Birthdate { get; set; }

    /// <summary>
    /// Player's music taste for personalization.
    /// </summary>
    public string MusicTaste { get; set; } = "";

    /// <summary>
    /// Player's film/movie preferences.
    /// </summary>
    public string FilmTaste { get; set; } = "";

    /// <summary>
    /// Player's video game preferences.
    /// </summary>
    public string GameTaste { get; set; } = "";

    /// <summary>
    /// Player's favorite programming language.
    /// </summary>
    public string FavoriteLanguage { get; set; } = "";

    /// <summary>
    /// Custom properties for game-specific personalization.
    /// </summary>
    public Dictionary<string, string> CustomProperties { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if today is the player's birthday.
    /// </summary>
    public bool IsBirthday()
    {
        if (Birthdate == null)
            return false;
        var today = DateOnly.FromDateTime(DateTime.Today);
        return Birthdate.Value.Month == today.Month && Birthdate.Value.Day == today.Day;
    }

    /// <summary>
    /// Gets the player's current age based on birthdate.
    /// </summary>
    public int? GetCalculatedAge()
    {
        if (Birthdate == null)
            return Age;
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - Birthdate.Value.Year;
        if (Birthdate.Value > today.AddYears(-age))
            age--;
        return age;
    }

    /// <summary>
    /// Saves the profile to a JSON file.
    /// </summary>
    public void Save(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var json = JsonSerializer.Serialize(this, JsonOptions);
        File.WriteAllText(path, json);
    }

    /// <summary>
    /// Loads a profile from a JSON file.
    /// </summary>
    public static UserProfile Load(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!File.Exists(path))
        {
            return new UserProfile();
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<UserProfile>(json, JsonOptions) ?? new UserProfile();
    }

    /// <summary>
    /// Loads profile from default location (~/.textadventure/profile.json).
    /// </summary>
    public static UserProfile LoadDefault()
    {
        var defaultPath = GetDefaultProfilePath();
        return Load(defaultPath);
    }

    /// <summary>
    /// Saves profile to default location (~/.textadventure/profile.json).
    /// </summary>
    public void SaveDefault()
    {
        var defaultPath = GetDefaultProfilePath();
        var directory = Path.GetDirectoryName(defaultPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            _ = Directory.CreateDirectory(directory);
        }

        Save(defaultPath);
    }

    /// <summary>
    /// Gets the default profile path.
    /// </summary>
    public static string GetDefaultProfilePath()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return Path.Combine(home, ".textadventure", "profile.json");
    }

    /// <summary>
    /// Determines the appropriate NPC gender for romance based on player preferences.
    /// </summary>
    /// <param name="originalNpcGender">The NPC's original gender in the story.</param>
    /// <returns>The gender the NPC should present as for this player.</returns>
    public Gender GetRomanceNpcGender(Gender originalNpcGender)
    {
        // If no preference set, use original
        if (RomancePreference == Gender.Unspecified)
        {
            return originalNpcGender;
        }

        // If preference is "any", use original
        if (RomancePreference == Gender.Any)
        {
            return originalNpcGender;
        }

        // Otherwise, use player's preference
        return RomancePreference;
    }
}

/// <summary>
/// Gender options for profile and NPCs.
/// </summary>
public enum Gender
{
    /// <summary>
    /// Not specified.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Male.
    /// </summary>
    Male = 1,

    /// <summary>
    /// Female.
    /// </summary>
    Female = 2,

    /// <summary>
    /// Non-binary.
    /// </summary>
    NonBinary = 3,

    /// <summary>
    /// Any gender (for romance preference).
    /// </summary>
    Any = 4
}

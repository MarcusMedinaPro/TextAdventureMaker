// <copyright file="UserProfileTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>


using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tests;

public class UserProfileTests
{
    [Fact]
    public void IsBirthday_WhenTodayIsBirthday_ReturnsTrue()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        UserProfile profile = new()
        {
            Birthdate = new DateOnly(1990, today.Month, today.Day)
        };

        Assert.True(profile.IsBirthday());
    }

    [Fact]
    public void IsBirthday_WhenNotBirthday_ReturnsFalse()
    {
        DateOnly tomorrow = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
        UserProfile profile = new()
        {
            Birthdate = new DateOnly(1990, tomorrow.Month, tomorrow.Day)
        };

        Assert.False(profile.IsBirthday());
    }

    [Fact]
    public void IsBirthday_WhenNoBirthdate_ReturnsFalse()
    {
        UserProfile profile = new();

        Assert.False(profile.IsBirthday());
    }

    [Fact]
    public void GetCalculatedAge_CalculatesCorrectly()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);
        int birthYear = today.Year - 30;
        UserProfile profile = new()
        {
            Birthdate = new DateOnly(birthYear, today.Month, today.Day)
        };

        Assert.Equal(30, profile.GetCalculatedAge());
    }

    [Fact]
    public void GetCalculatedAge_WhenNoBirthdate_ReturnsAge()
    {
        UserProfile profile = new() { Age = 25 };

        Assert.Equal(25, profile.GetCalculatedAge());
    }

    [Fact]
    public void GetRomanceNpcGender_WhenUnspecified_ReturnsOriginal()
    {
        UserProfile profile = new()
        {
            RomancePreference = Gender.Unspecified
        };

        Assert.Equal(Gender.Female, profile.GetRomanceNpcGender(Gender.Female));
        Assert.Equal(Gender.Male, profile.GetRomanceNpcGender(Gender.Male));
    }

    [Fact]
    public void GetRomanceNpcGender_WhenAny_ReturnsOriginal()
    {
        UserProfile profile = new()
        {
            RomancePreference = Gender.Any
        };

        Assert.Equal(Gender.Female, profile.GetRomanceNpcGender(Gender.Female));
        Assert.Equal(Gender.Male, profile.GetRomanceNpcGender(Gender.Male));
    }

    [Fact]
    public void GetRomanceNpcGender_WhenPreferenceSet_ReturnsPreference()
    {
        UserProfile profile = new()
        {
            RomancePreference = Gender.Male
        };

        Assert.Equal(Gender.Male, profile.GetRomanceNpcGender(Gender.Female));
        Assert.Equal(Gender.Male, profile.GetRomanceNpcGender(Gender.Male));
    }

    [Fact]
    public void SaveAndLoad_RoundTrip_PreservesData()
    {
        string tempFile = Path.GetTempFileName();
        try
        {
            UserProfile original = new()
            {
                Name = "Test Player",
                Gender = Gender.Female,
                RomancePreference = Gender.Male,
                HairColor = "Brown",
                EyeColor = "Green",
                Age = 28,
                Birthdate = new DateOnly(1996, 5, 15),
                MusicTaste = "Rock",
                FilmTaste = "Sci-Fi",
                GameTaste = "RPG",
                FavoriteLanguage = "C#"
            };
            original.CustomProperties["Theme"] = "Dark";

            original.Save(tempFile);
            UserProfile loaded = UserProfile.Load(tempFile);

            Assert.Equal(original.Name, loaded.Name);
            Assert.Equal(original.Gender, loaded.Gender);
            Assert.Equal(original.RomancePreference, loaded.RomancePreference);
            Assert.Equal(original.HairColor, loaded.HairColor);
            Assert.Equal(original.EyeColor, loaded.EyeColor);
            Assert.Equal(original.Age, loaded.Age);
            Assert.Equal(original.Birthdate, loaded.Birthdate);
            Assert.Equal(original.MusicTaste, loaded.MusicTaste);
            Assert.Equal(original.FilmTaste, loaded.FilmTaste);
            Assert.Equal(original.GameTaste, loaded.GameTaste);
            Assert.Equal(original.FavoriteLanguage, loaded.FavoriteLanguage);
            Assert.Equal("Dark", loaded.CustomProperties["Theme"]);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void Load_WhenFileDoesNotExist_ReturnsNewProfile()
    {
        UserProfile profile = UserProfile.Load("/nonexistent/path/profile.json");

        Assert.NotNull(profile);
        Assert.Equal("", profile.Name);
    }
}

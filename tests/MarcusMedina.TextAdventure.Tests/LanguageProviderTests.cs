// <copyright file="LanguageProviderTests.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Tests;

public class LanguageProviderTests
{
    [Fact]
    public void FileLanguageProvider_LoadsKeys()
    {
        string file = Path.GetTempFileName();
        try
        {
            File.WriteAllText(file, "hello=Hej\nDoorLockedTemplate=Låst: {0}\n");
            FileLanguageProvider provider = new(file);

            Assert.Equal("Hej", provider.Get("hello"));
            Assert.Equal("Låst: dörr", provider.Format("DoorLockedTemplate", "dörr"));
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Fact]
    public void FileLanguageProvider_IgnoresCommentsAndEmptyLines()
    {
        string file = Path.GetTempFileName();
        try
        {
            File.WriteAllText(file, "# comment\n\nkey=value\n");
            FileLanguageProvider provider = new(file);

            Assert.Equal("value", provider.Get("key"));
        }
        finally
        {
            File.Delete(file);
        }
    }
}

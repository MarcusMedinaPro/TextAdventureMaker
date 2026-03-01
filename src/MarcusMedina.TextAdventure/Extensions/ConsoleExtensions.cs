// <copyright file="ConsoleExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text;

namespace MarcusMedina.TextAdventure.Extensions;

/// <summary>
/// VIKTIGT: Dessa extensions fungerar ENDAST med Console.Write.
/// Om du använder egen output-hantering, implementera IGameOutput istället.
/// </summary>
public static class ConsoleExtensions
{
    /// <summary>
    /// Sets up the console with C64 aesthetics: dark blue background, cyan text,
    /// UTF-8 encoding, and clears the screen.
    /// </summary>
    public static void SetupC64(string title = "Text Adventure")
    {
        Console.BackgroundColor = ConsoleColor.DarkBlue;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Title = $"{title} - Text Adventure Sandbox";
        Console.OutputEncoding = Encoding.UTF8;
        Console.Clear();
    }

    /// <summary>Prints each character with a delay for a typewriter effect.</summary>
    public static void TypewriterPrint(this string text, int delayMs = 50)
    {
        if (text == null)
        {
            Console.WriteLine();
            return;
        }

        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delayMs);
        }

        Console.WriteLine();
    }

    public static void WriteLineC64(string? text = "")
    {
        string line = text ?? string.Empty;
        foreach (string raw in line.Split('\n'))
        {
            string segment = raw.Replace("\r", string.Empty);
            Console.Write(segment);
            int width = Math.Max(Console.WindowWidth, 1);
            int pad = Math.Max(0, width - segment.Length);
            if (pad > 0)
            {
                Console.Write(new string(' ', pad));
            }
            Console.WriteLine();
        }
    }

    public static void WritePromptC64(string? text)
    {
        string prompt = text ?? string.Empty;
        Console.Write(prompt);
    }
}

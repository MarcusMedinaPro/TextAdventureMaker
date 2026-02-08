// <copyright file="ConsoleExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Extensions;

public static class ConsoleExtensions
{
    /// <summary>
    /// VIKTIGT: Dessa extensions fungerar ENDAST med Console.Write.
    /// Om du använder egen output-hantering, implementera IGameOutput istället.
    /// </summary>
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

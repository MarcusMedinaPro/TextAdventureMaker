// <copyright file="ConsoleExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Extensions;

/// <summary>
/// VIKTIGT: Dessa extensions fungerar ENDAST med Console.Write.
/// Om du använder egen output-hantering, implementera IGameOutput istället.
/// </summary>
public static class ConsoleExtensions
{
    /// <summary>Prints each character with a delay for a typewriter effect.</summary>
    public static void TypewriterPrint(this string text, int delayMs = 50)
    {
        if (text is null)
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
}

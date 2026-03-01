// <copyright file="TypewriterPresenter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Presents text with typewriter effect and dramatic timing.
/// </summary>
public sealed class TypewriterPresenter : ITextPresenter
{
    private TextSpeed _speed = TextSpeed.Normal;

    public void Present(string text, PresentationOptions? options = null)
    {
        options ??= new PresentationOptions();

        // Pause before
        if (options.PauseBefore.HasValue)
            System.Threading.Thread.Sleep((int)options.PauseBefore.Value.TotalMilliseconds);

        // Set color if specified
        var originalColor = Console.ForegroundColor;
        if (options.Color.HasValue)
            Console.ForegroundColor = options.Color.Value;

        var delay = GetDelay(options.Speed);

        if (delay == 0)
        {
            // Instant presentation
            Console.Write(text);
        }
        else
        {
            // Typewriter effect
            foreach (var ch in text)
            {
                Console.Write(ch);
                System.Threading.Thread.Sleep(delay);
            }
        }

        // Reset color
        Console.ForegroundColor = originalColor;

        // Pause after
        if (options.PauseAfter.HasValue)
            System.Threading.Thread.Sleep((int)options.PauseAfter.Value.TotalMilliseconds);
    }

    public void SetSpeed(TextSpeed speed) => _speed = speed;

    private static int GetDelay(TextSpeed speed) =>
        speed switch
        {
            TextSpeed.Instant => 0,
            TextSpeed.Fast => 10,
            TextSpeed.Normal => 30,
            TextSpeed.Slow => 50,
            TextSpeed.Dramatic => 100,
            _ => 30
        };
}

// <copyright file="ScenePresenter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Manages cinematic scene presentation with multiple text elements.
/// </summary>
public sealed class ScenePresenter
{
    private readonly ITextPresenter _presenter;

    public ScenePresenter(ITextPresenter? presenter = null)
    {
        _presenter = presenter ?? new TypewriterPresenter();
    }

    /// <summary>
    /// Presents a dramatic scene with description and action.
    /// </summary>
    public void PresentScene(string title, string description, string? action = null)
    {
        Console.Clear();

        // Title
        _presenter.Present($"═══ {title} ═══\n", new PresentationOptions(Speed: TextSpeed.Instant));

        // Description
        _presenter.Present(description + "\n\n", new PresentationOptions(Speed: TextSpeed.Normal));

        // Action
        if (!string.IsNullOrEmpty(action))
        {
            System.Threading.Thread.Sleep(500);
            _presenter.Present(action + "\n", new PresentationOptions(Speed: TextSpeed.Slow, Color: ConsoleColor.Yellow));
        }

        System.Threading.Thread.Sleep(1000);
    }

    /// <summary>
    /// Presents ASCII art with optional description.
    /// </summary>
    public void PresentAsciiArt(string ascii, PresentationOptions? options = null)
    {
        Console.Clear();
        _presenter.Present(ascii, options ?? new PresentationOptions(Speed: TextSpeed.Instant));
        Console.WriteLine();
    }

    /// <summary>
    /// Presents a dramatic pause with ellipsis.
    /// </summary>
    public void PresentPause(int dots = 3, int delayMs = 500)
    {
        for (int i = 0; i < dots; i++)
        {
            Console.Write(".");
            System.Threading.Thread.Sleep(delayMs);
        }
        Console.WriteLine();
    }
}

// <copyright file="ITextPresenter.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

/// <summary>
/// Controls the speed of typewriter text presentation.
/// </summary>
public enum TextSpeed
{
    Instant,        // All at once
    Fast,           // 10ms per character
    Normal,         // 30ms per character
    Slow,           // 50ms per character
    Dramatic        // 100ms per character
}

/// <summary>
/// Options for text presentation effects.
/// </summary>
public sealed record PresentationOptions(
    TextSpeed Speed = TextSpeed.Normal,
    bool AllowSkip = true,
    TimeSpan? PauseBefore = null,
    TimeSpan? PauseAfter = null,
    ConsoleColor? Color = null
);

/// <summary>
/// Interface for dramatic text presentation with effects like typewriter.
/// </summary>
public interface ITextPresenter
{
    /// <summary>
    /// Presents text with cinematic effects.
    /// </summary>
    void Present(string text, PresentationOptions? options = null);

    /// <summary>
    /// Sets the default text speed.
    /// </summary>
    void SetSpeed(TextSpeed speed);
}

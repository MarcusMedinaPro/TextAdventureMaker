// <copyright file="IAccessibilitySystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Interfaces;

public enum VerbosityLevel
{
    Brief,
    Normal,
    Verbose
}

public enum ContrastLevel
{
    Normal,
    High,
    Highest
}

public interface IAccessibilitySystem
{
    VerbosityLevel Verbosity { get; }
    ContrastLevel Contrast { get; }
    bool ScreenReaderEnabled { get; }

    IAccessibilitySystem SetVerbosity(VerbosityLevel level);
    IAccessibilitySystem EnableScreenReader(bool enabled = true);
    IAccessibilitySystem SetContrast(ContrastLevel level);
}

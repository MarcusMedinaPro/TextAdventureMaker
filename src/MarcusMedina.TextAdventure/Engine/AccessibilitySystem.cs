// <copyright file="AccessibilitySystem.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Engine;

public class AccessibilitySystem : IAccessibilitySystem
{
    private VerbosityLevel _verbosity = VerbosityLevel.Normal;
    private ContrastLevel _contrast = ContrastLevel.Normal;
    private bool _screenReaderEnabled;

    public VerbosityLevel Verbosity => _verbosity;
    public ContrastLevel Contrast => _contrast;
    public bool ScreenReaderEnabled => _screenReaderEnabled;

    public IAccessibilitySystem SetVerbosity(VerbosityLevel level)
    {
        _verbosity = level;
        return this;
    }

    public IAccessibilitySystem EnableScreenReader(bool enabled = true)
    {
        _screenReaderEnabled = enabled;
        return this;
    }

    public IAccessibilitySystem SetContrast(ContrastLevel level)
    {
        _contrast = level;
        return this;
    }
}

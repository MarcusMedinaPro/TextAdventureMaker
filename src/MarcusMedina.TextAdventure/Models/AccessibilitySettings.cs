// <copyright file="AccessibilitySettings.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

public class AccessibilitySettings(VerbosityLevel verbosity = VerbosityLevel.Normal, bool screenReaderEnabled = false, ContrastLevel contrast = ContrastLevel.Normal)
{
    public VerbosityLevel Verbosity { get; set; } = verbosity;
    public bool ScreenReaderEnabled { get; set; } = screenReaderEnabled;
    public ContrastLevel Contrast { get; set; } = contrast;
}

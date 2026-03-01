// <copyright file="AccessibilityCommand.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Commands;

public class AccessibilityCommand(string? option = null) : ICommand
{
    public string Name => "accessibility";
    public string[]? Aliases => ["a11y"];
    public string Description => "Configure accessibility settings (verbosity, screen reader, contrast)";

    public CommandResult Execute(CommandContext context)
    {
        IAccessibilitySystem? accessibility = context.State.Accessibility;
        if (accessibility  is null)
            return CommandResult.Fail("Accessibility system is not enabled.", Enums.GameError.None);

        if (string.IsNullOrWhiteSpace(option))
            return ShowStatus(accessibility);

        string[] parts = option.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return ShowStatus(accessibility);

        string command = parts[0].ToLowerInvariant();

        return command switch
        {
            "verbosity" => HandleVerbosity(accessibility, parts),
            "verbose" => HandleVerbosityShort(accessibility, "verbose"),
            "brief" => HandleVerbosityShort(accessibility, "brief"),
            "normal" => HandleVerbosityShort(accessibility, "normal"),
            "screenreader" => HandleScreenReader(accessibility, parts),
            "contrast" => HandleContrast(accessibility, parts),
            "status" => ShowStatus(accessibility),
            "help" => ShowHelp(),
            _ => CommandResult.Fail($"Unknown accessibility option '{command}'. Type 'accessibility help' for options.", Enums.GameError.None)
        };
    }

    private CommandResult HandleVerbosity(IAccessibilitySystem accessibility, string[] parts)
    {
        if (parts.Length < 2)
            return CommandResult.Fail("Verbosity requires a level: brief, normal, or verbose.", Enums.GameError.None);

        string level = parts[1].ToLowerInvariant();
        VerbosityLevel? verbosity = level switch
        {
            "brief" => VerbosityLevel.Brief,
            "normal" => VerbosityLevel.Normal,
            "verbose" => VerbosityLevel.Verbose,
            _ => null
        };

        if (verbosity  is null)
            return CommandResult.Fail($"Unknown verbosity level '{level}'. Use: brief, normal, or verbose.", Enums.GameError.None);

        _ = accessibility.SetVerbosity(verbosity.Value);
        return CommandResult.Ok(
            $"Verbosity set to {level}.",
            $"Accessibility verbosity is now {level}.");
    }

    private CommandResult HandleVerbosityShort(IAccessibilitySystem accessibility, string level)
    {
        VerbosityLevel? verbosity = level switch
        {
            "brief" => VerbosityLevel.Brief,
            "normal" => VerbosityLevel.Normal,
            "verbose" => VerbosityLevel.Verbose,
            _ => null
        };

        if (verbosity  is null)
            return CommandResult.Fail($"Unknown verbosity level '{level}'.", Enums.GameError.None);

        _ = accessibility.SetVerbosity(verbosity.Value);
        return CommandResult.Ok(
            $"Verbosity set to {level}.",
            $"Accessibility verbosity is now {level}.");
    }

    private CommandResult HandleScreenReader(IAccessibilitySystem accessibility, string[] parts)
    {
        if (parts.Length < 2)
        {
            bool currentState = accessibility.ScreenReaderEnabled;
            return CommandResult.Ok($"Screen reader is currently {(currentState ? "enabled" : "disabled")}.");
        }

        string state = parts[1].ToLowerInvariant();
        bool? enabled = state switch
        {
            "on" or "enable" or "yes" or "true" => true,
            "off" or "disable" or "no" or "false" => false,
            _ => null
        };

        if (enabled  is null)
            return CommandResult.Fail($"Use: screenreader on/off", Enums.GameError.None);

        _ = accessibility.EnableScreenReader(enabled.Value);
        return CommandResult.Ok(
            $"Screen reader {(enabled.Value ? "enabled" : "disabled")}.",
            $"Screen reader is now {(enabled.Value ? "enabled" : "disabled")}.");
    }

    private CommandResult HandleContrast(IAccessibilitySystem accessibility, string[] parts)
    {
        if (parts.Length < 2)
            return CommandResult.Fail("Contrast requires a level: normal, high, or highest.", Enums.GameError.None);

        string level = parts[1].ToLowerInvariant();
        ContrastLevel? contrast = level switch
        {
            "normal" => ContrastLevel.Normal,
            "high" => ContrastLevel.High,
            "highest" => ContrastLevel.Highest,
            _ => null
        };

        if (contrast  is null)
            return CommandResult.Fail($"Unknown contrast level '{level}'. Use: normal, high, or highest.", Enums.GameError.None);

        _ = accessibility.SetContrast(contrast.Value);
        return CommandResult.Ok(
            $"Contrast set to {level}.",
            $"Accessibility contrast is now {level}.");
    }

    private CommandResult ShowStatus(IAccessibilitySystem accessibility)
    {
        string verbosity = accessibility.Verbosity.ToString().ToLowerInvariant();
        string contrast = accessibility.Contrast.ToString().ToLowerInvariant();
        string screenReader = accessibility.ScreenReaderEnabled ? "enabled" : "disabled";

        return CommandResult.Ok(
            $"Verbosity: {verbosity} | Screen Reader: {screenReader} | Contrast: {contrast}");
    }

    private CommandResult ShowHelp()
    {
        var help = new System.Text.StringBuilder();
        help.AppendLine("Accessibility Commands:");
        help.AppendLine("  accessibility status - Show current settings");
        help.AppendLine("  accessibility verbosity [brief|normal|verbose] - Set detail level");
        help.AppendLine("  accessibility screenreader [on|off] - Toggle screen reader mode");
        help.AppendLine("  accessibility contrast [normal|high|highest] - Set contrast level");
        help.AppendLine("  accessibility help - Show this help");

        return CommandResult.Ok(help.ToString());
    }
}

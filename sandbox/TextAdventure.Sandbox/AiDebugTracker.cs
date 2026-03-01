using MarcusMedina.TextAdventure.Commands;
using MarcusMedina.TextAdventure.Interfaces;

internal sealed class AiDebugTracker(bool enabled = false)
{
    private readonly List<AiDebugEvent> _events = [];
    private readonly Lock _sync = new();

    public bool Enabled { get; private set; } = enabled;

    public void StartTurn()
    {
        lock (_sync)
        {
            _events.Clear();
        }
    }

    public void Record(string eventName, string payload)
    {
        if (string.IsNullOrWhiteSpace(eventName))
            return;

        lock (_sync)
        {
            _events.Add(new AiDebugEvent(eventName.Trim(), Compact(payload)));
        }
    }

    public string Toggle()
    {
        bool next = !Enabled;
        Enabled = next;
        return next ? "AI debug is now ON." : "AI debug is now OFF.";
    }

    public string Set(bool enabledState)
    {
        Enabled = enabledState;
        return enabledState ? "AI debug is now ON." : "AI debug is now OFF.";
    }

    public string Status()
    {
        return Enabled ? "AI debug is ON." : "AI debug is OFF.";
    }

    public IReadOnlyList<string> BuildTurnLines()
    {
        List<AiDebugEvent> snapshot;
        lock (_sync)
        {
            snapshot = [.. _events];
        }

        if (snapshot.Count == 0)
            return [];

        bool usedAi = snapshot.Any(static e => e.IsAiUseEvent);
        bool usedCache = snapshot.Any(static e => e.IsCacheHitEvent);
        List<string> lines = [];

        if (usedAi && usedCache)
            lines.Add("[AI, Cache]");
        else if (usedAi)
            lines.Add("[AI]");
        else if (usedCache)
            lines.Add("[Cache]");

        if (!Enabled)
            return lines;

        foreach (AiDebugEvent entry in snapshot)
        {
            string? line = entry.Name switch
            {
                "parser.ai.call" or "feature.ai.call" => $"[AI call: {entry.Payload}]",
                "parser.ai.response" or "feature.ai.response" => $"[AI: {entry.Payload}]",
                "description.cache.hit" => $"[Cache: {entry.Payload}]",
                _ => null
            };

            if (!string.IsNullOrWhiteSpace(line))
                lines.Add(line);
        }

        return lines;
    }

    private static string Compact(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "(empty)";

        const int max = 220;
        string singleLine = value.ReplaceLineEndings(" ").Trim();
        return singleLine.Length <= max ? singleLine : $"{singleLine[..max]}...";
    }

    private sealed record AiDebugEvent(string Name, string Payload)
    {
        public bool IsAiUseEvent => Name is "parser.ai.call" or "feature.ai.call";
        public bool IsCacheHitEvent => Name == "description.cache.hit";
    }
}

internal sealed class SandboxDebugCommandParser(ICommandParser inner, AiDebugTracker tracker) : ICommandParser
{
    private readonly ICommandParser _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    private readonly AiDebugTracker _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));

    public ICommand Parse(string input)
    {
        if (DebugToggleModeParser.TryParse(input, out DebugToggleMode mode))
            return new DebugToggleCommand(_tracker, mode);

        return _inner.Parse(input);
    }
}

internal sealed class DebugToggleCommand(AiDebugTracker tracker, DebugToggleMode mode) : ICommand
{
    private readonly AiDebugTracker _tracker = tracker ?? throw new ArgumentNullException(nameof(tracker));
    private readonly DebugToggleMode _mode = mode;

    public CommandResult Execute(CommandContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        string message = _mode switch
        {
            DebugToggleMode.Toggle => _tracker.Toggle(),
            DebugToggleMode.On => _tracker.Set(true),
            DebugToggleMode.Off => _tracker.Set(false),
            DebugToggleMode.Status => _tracker.Status(),
            _ => _tracker.Status()
        };

        return CommandResult.Ok(message);
    }
}

internal enum DebugToggleMode
{
    Toggle,
    On,
    Off,
    Status
}

internal static class DebugToggleModeParser
{
    public static bool TryParse(string? input, out DebugToggleMode mode)
    {
        mode = DebugToggleMode.Toggle;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        string trimmed = input.Trim();
        if (!trimmed.StartsWith("/debug", StringComparison.OrdinalIgnoreCase))
            return false;

        if (trimmed.Equals("/debug", StringComparison.OrdinalIgnoreCase))
        {
            mode = DebugToggleMode.Toggle;
            return true;
        }

        string arg = trimmed["/debug".Length..].Trim();
        mode = arg.ToLowerInvariant() switch
        {
            "on" => DebugToggleMode.On,
            "off" => DebugToggleMode.Off,
            "status" => DebugToggleMode.Status,
            _ => DebugToggleMode.Toggle
        };

        return true;
    }
}

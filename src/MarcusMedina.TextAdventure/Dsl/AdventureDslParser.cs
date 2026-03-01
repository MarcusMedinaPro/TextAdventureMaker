// <copyright file="AdventureDslParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;
using System.Globalization;

namespace MarcusMedina.TextAdventure.Dsl;

public class AdventureDslParser : IDslParser
{
    private readonly Dictionary<string, Action<AdventureDslContext, string>> _handlers =
        new(StringComparer.OrdinalIgnoreCase);

    public AdventureDslParser()
    {
        RegisterDefaultKeywords();
    }

    public AdventureDslParser RegisterKeyword(string keyword, Action<AdventureDslContext, string> handler)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyword);
        ArgumentNullException.ThrowIfNull(handler);
        _handlers[keyword.Trim()] = handler;
        return this;
    }

    public DslAdventure ParseFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return ParseLines(File.ReadLines(path));
    }

    public DslAdventure ParseString(string adventureText)
    {
        ArgumentNullException.ThrowIfNull(adventureText);
        string[] lines = adventureText.Split('\n');
        return ParseLines(lines);
    }

    private DslAdventure ParseLines(IEnumerable<string> lines)
    {
        AdventureDslContext context = CreateContext();
        OnBeforeParse(context);

        int lineNumber = 0;
        foreach (string line in lines)
        {
            lineNumber++;
            context.CurrentLineNumber = lineNumber;
            context.CurrentLineContent = line;

            (string Keyword, string Value)? parsedLine = ParseLine(line);
            if (parsedLine == null)
                continue;

            Dispatch(context, parsedLine.Value.Keyword, parsedLine.Value.Value, lineNumber, line);
        }

        OnAfterParse(context);

        return BuildAdventure(context);
    }

    protected virtual AdventureDslContext CreateContext()
    {
        return new();
    }

    protected virtual void OnBeforeParse(AdventureDslContext context)
    {
    }

    protected virtual void OnAfterParse(AdventureDslContext context)
    {
    }

    protected virtual (string Keyword, string Value)? ParseLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return null;
        }

        string trimmed = line.Trim();
        if (trimmed.StartsWith("#", StringComparison.Ordinal))
        {
            return null;
        }

        if (trimmed.StartsWith("//", StringComparison.Ordinal))
        {
            return null;
        }

        int separatorIndex = trimmed.IndexOf(':');
        if (separatorIndex <= 0)
        {
            return null;
        }

        string keyword = trimmed[..separatorIndex].Trim();
        string value = trimmed[(separatorIndex + 1)..].Trim();
        return keyword.Length == 0 ? null : (keyword, value);
    }

    private void Dispatch(AdventureDslContext context, string keyword, string value, int lineNumber, string lineContent)
    {
        if (_handlers.TryGetValue(keyword, out Action<AdventureDslContext, string>? handler))
        {
            handler(context, value);
            return;
        }

        context.AddWarning(DslErrorHelper.UnknownKeyword(lineNumber, lineContent.Trim(), keyword));
    }

    private void RegisterDefaultKeywords()
    {
        _ = RegisterKeyword("world", (ctx, value) => ctx.SetMetadata("world", value));
        _ = RegisterKeyword("goal", (ctx, value) => ctx.SetMetadata("goal", value));
        _ = RegisterKeyword("start", (ctx, value) => ctx.StartLocationId = NormalizeId(value));
        _ = RegisterKeyword("location", HandleLocation);
        _ = RegisterKeyword("description", HandleDescription);
        _ = RegisterKeyword("item", HandleItem);
        _ = RegisterKeyword("key", HandleKey);
        _ = RegisterKeyword("door", HandleDoor);
        _ = RegisterKeyword("exit", HandleExit);
        _ = RegisterKeyword("timed_spawn", HandleTimedSpawn);
        _ = RegisterKeyword("timed_door", HandleTimedDoor);
    }

    private static void HandleLocation(AdventureDslContext context, string value)
    {
        List<string> parts = SplitParts(value);
        if (parts.Count == 0)
        {
            return;
        }

        string id = NormalizeId(parts[0]);
        string? description = parts.Count > 1 ? parts[1] : null;
        context.SetCurrentLocation(id, description);
    }

    private static void HandleDescription(AdventureDslContext context, string value)
    {
        context.RequireCurrentLocation();
        _ = context.CurrentLocation!.Description(value);
    }

    private static void HandleItem(AdventureDslContext context, string value)
    {
        context.RequireCurrentLocation();

        DslItemParts parsed = ParseItemParts(value);
        Item item = context.GetOrCreateItem(parsed.Id, parsed.Name, parsed.Description);

        ApplyItemOptions(item, parsed.Options);
        context.CurrentLocation!.AddItem(item);
    }

    private static void HandleKey(AdventureDslContext context, string value)
    {
        context.RequireCurrentLocation();

        DslItemParts parsed = ParseItemParts(value);
        Key key = context.GetOrCreateKey(parsed.Id, parsed.Name, parsed.Description);

        ApplyItemOptions(key, parsed.Options);
        context.CurrentLocation!.AddItem(key);
    }

    private static void HandleDoor(AdventureDslContext context, string value)
    {
        DslDoorParts parsed = ParseDoorParts(value);
        Door door = context.GetOrCreateDoor(parsed.Id, parsed.Name, parsed.Description);

        foreach (KeyValuePair<string, string> option in parsed.Options)
        {
            if (option.Key.TextCompare("key"))
            {
                string keyId = NormalizeId(option.Value);
                if (context.Keys.TryGetValue(keyId, out Key? key))
                {
                    _ = door.RequiresKey(key);
                }
                else
                {
                    context.PendingDoorKeys.Add(new PendingDoorKey(parsed.Id, keyId, context.CurrentLineNumber, context.CurrentLineContent));
                }
            }
        }
    }

    private static void HandleExit(AdventureDslContext context, string value)
    {
        context.RequireCurrentLocation();

        List<string> parts = SplitParts(value);
        if (parts.Count == 0)
        {
            return;
        }

        string route = parts[0];
        int arrowIndex = route.IndexOf("->", StringComparison.Ordinal);
        if (arrowIndex <= 0)
        {
            return;
        }

        string directionText = route[..arrowIndex].Trim();
        string targetText = route[(arrowIndex + 2)..].Trim();

        if (!TryParseDirection(directionText, out Direction direction))
        {
            return;
        }

        string targetId = NormalizeId(targetText);

        string? doorId = null;
        bool oneWay = false;

        for (int i = 1; i < parts.Count; i++)
        {
            KeyValuePair<string, string>? option = ParseOption(parts[i]);
            if (option == null)
            {
                continue;
            }

            if (option.Value.Key.TextCompare("door"))
            {
                doorId = NormalizeId(option.Value.Value);
            }
            else if (option.Value.Key.TextCompare("oneway") || option.Value.Key.TextCompare("one-way"))
            {
                oneWay = true;
            }
        }

        context.PendingExits.Add(new PendingExit(
            context.CurrentLocation!.Id,
            targetId,
            direction,
            doorId,
            oneWay,
            context.CurrentLineNumber,
            context.CurrentLineContent));
    }

    private static void HandleTimedSpawn(AdventureDslContext context, string value)
    {
        context.RequireCurrentLocation();

        List<string> parts = SplitParts(value);
        if (parts.Count == 0)
            return;

        string itemId = NormalizeId(parts[0]);
        TimedSpawn spawn = context.CurrentLocation!.AddTimedSpawn(itemId);

        for (int i = 1; i < parts.Count; i++)
        {
            KeyValuePair<string, string>? option = ParseOption(parts[i]);
            if (option == null)
                continue;

            string key = option.Value.Key;
            string val = option.Value.Value;

            if (key.TextCompare("appears_at"))
                ApplyTickOrPhase(val, tick => spawn.AppearsAt(tick), phase => spawn.AppearsAt(phase));
            else if (key.TextCompare("disappears_after") && int.TryParse(val, out int ticks))
                _ = spawn.DisappearsAfter(ticks);
            else if (key.TextCompare("disappears_at") && Enum.TryParse<TimePhase>(val, true, out TimePhase phase))
                _ = spawn.DisappearsAt(phase);
            else if (key.TextCompare("message"))
                _ = spawn.Message(val);
        }
    }

    private static void HandleTimedDoor(AdventureDslContext context, string value)
    {
        context.RequireCurrentLocation();

        List<string> parts = SplitParts(value);
        if (parts.Count == 0)
            return;

        string directionText = parts[0].Trim();
        if (!TryParseDirection(directionText, out Direction direction))
            return;

        if (!context.CurrentLocation!.Exits.TryGetValue(direction, out Exit? exit))
            return;

        string doorId = $"timed_{context.CurrentLocation.Id}_{directionText}".ToLowerInvariant();
        TimedDoor timedDoor = exit.WithTimedDoor(doorId);

        for (int i = 1; i < parts.Count; i++)
        {
            KeyValuePair<string, string>? option = ParseOption(parts[i]);
            if (option == null)
                continue;

            string key = option.Value.Key;
            string val = option.Value.Value;

            if (key.TextCompare("opens_at"))
                ApplyTickOrPhase(val, tick => timedDoor.OpensAt(tick), phase => timedDoor.OpensAt(phase));
            else if (key.TextCompare("closes_at"))
                ApplyTickOrPhase(val, tick => timedDoor.ClosesAt(tick), phase => timedDoor.ClosesAt(phase));
            else if (key.TextCompare("message"))
                _ = timedDoor.Message(val);
            else if (key.TextCompare("closed_message"))
                _ = timedDoor.ClosedMessage(val);
        }
    }

    private static void ApplyTickOrPhase(string value, Action<int> onTick, Action<TimePhase> onPhase)
    {
        if (int.TryParse(value, out int tick))
        {
            onTick(tick);
            return;
        }

        if (Enum.TryParse<TimePhase>(value, true, out TimePhase phase))
            onPhase(phase);
    }

    private static DslItemParts ParseItemParts(string value)
    {
        List<string> parts = SplitParts(value);
        if (parts.Count == 0)
        {
            return new DslItemParts("item", "item", null, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
        }

        string id;
        string name;
        string? description = null;
        int optionStart;
        if (parts.Count >= 3)
        {
            id = NormalizeId(parts[0]);
            name = parts[1].Trim();
            description = parts[2].Trim();
            optionStart = 3;
        }
        else if (parts.Count == 2)
        {
            name = parts[0].Trim();
            id = NormalizeId(name);
            description = parts[1].Trim();
            optionStart = 2;
        }
        else
        {
            name = parts[0].Trim();
            id = NormalizeId(name);
            optionStart = 1;
        }

        IReadOnlyDictionary<string, string> options = ParseOptions(parts, optionStart);
        return new DslItemParts(id, name, description, options);
    }

    private static DslDoorParts ParseDoorParts(string value)
    {
        List<string> parts = SplitParts(value);
        if (parts.Count == 0)
        {
            return new DslDoorParts("door", "door", null, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
        }

        string id;
        string name;
        string? description = null;
        int optionStart;
        if (parts.Count >= 3)
        {
            id = NormalizeId(parts[0]);
            name = parts[1].Trim();
            description = parts[2].Trim();
            optionStart = 3;
        }
        else if (parts.Count == 2)
        {
            name = parts[0].Trim();
            id = NormalizeId(name);
            description = parts[1].Trim();
            optionStart = 2;
        }
        else
        {
            name = parts[0].Trim();
            id = NormalizeId(name);
            optionStart = 1;
        }

        IReadOnlyDictionary<string, string> options = ParseOptions(parts, optionStart);
        return new DslDoorParts(id, name, description, options);
    }

    private static void ApplyItemOptions(Item item, IReadOnlyDictionary<string, string> options)
    {
        foreach (KeyValuePair<string, string> option in options)
        {
            if (option.Key.TextCompare("weight") &&
                float.TryParse(option.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float weight))
            {
                _ = item.SetWeight(weight);
            }
            else if (option.Key.TextCompare("aliases"))
            {
                string[] aliases = option.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                _ = item.AddAliases(aliases);
            }
            else if (option.Key.TextCompare("takeable") && bool.TryParse(option.Value, out bool takeable))
            {
                _ = item.SetTakeable(takeable);
            }
        }
    }

    private static void ApplyItemOptions(Key key, IReadOnlyDictionary<string, string> options)
    {
        foreach (KeyValuePair<string, string> option in options)
        {
            if (option.Key.TextCompare("weight") &&
                float.TryParse(option.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out float weight))
            {
                _ = key.SetWeight(weight);
            }
            else if (option.Key.TextCompare("aliases"))
            {
                string[] aliases = option.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                _ = key.AddAliases(aliases);
            }
            else if (option.Key.TextCompare("takeable") && bool.TryParse(option.Value, out bool takeable))
            {
                _ = key.SetTakeable(takeable);
            }
        }
    }

    private static IReadOnlyDictionary<string, string> ParseOptions(IReadOnlyList<string> parts, int startIndex)
    {
        if (startIndex >= parts.Count)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        Dictionary<string, string> options = new(StringComparer.OrdinalIgnoreCase);
        for (int i = startIndex; i < parts.Count; i++)
        {
            KeyValuePair<string, string>? option = ParseOption(parts[i]);
            if (option == null)
            {
                continue;
            }

            options[option.Value.Key] = option.Value.Value;
        }

        return options;
    }

    private static KeyValuePair<string, string>? ParseOption(string raw)
    {
        string trimmed = raw.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return null;
        }

        int equalsIndex = trimmed.IndexOf('=');
        if (equalsIndex <= 0)
        {
            return new KeyValuePair<string, string>(trimmed, "");
        }

        string key = trimmed[..equalsIndex].Trim();
        string value = trimmed[(equalsIndex + 1)..].Trim();
        return new KeyValuePair<string, string>(key, value);
    }

    private static List<string> SplitParts(string value)
    {
        return value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }

    private static string NormalizeId(string value)
    {
        return value.Trim().ToId();
    }

    private static bool TryParseDirection(string value, out Direction direction)
    {
        return Enum.TryParse(value, true, out direction) || value.ToLowerInvariant() switch
        {
            "n" => AssignDirection(Direction.North, out direction),
            "s" => AssignDirection(Direction.South, out direction),
            "e" => AssignDirection(Direction.East, out direction),
            "w" => AssignDirection(Direction.West, out direction),
            "ne" => AssignDirection(Direction.NorthEast, out direction),
            "nw" => AssignDirection(Direction.NorthWest, out direction),
            "se" => AssignDirection(Direction.SouthEast, out direction),
            "sw" => AssignDirection(Direction.SouthWest, out direction),
            "u" => AssignDirection(Direction.Up, out direction),
            "d" => AssignDirection(Direction.Down, out direction),
            _ => AssignDirection(Direction.North, out direction, false)
        };
    }

    private static bool AssignDirection(Direction direction, out Direction output, bool success = true)
    {
        output = direction;
        return success;
    }

    private static DslAdventure BuildAdventure(AdventureDslContext context)
    {
        ResolveDoorKeys(context);
        ResolveExits(context);

        Location startLocation = ResolveStartLocation(context);
        List<Location> locations = context.Locations.Values.ToList();
        GameState state = new(startLocation, worldLocations: locations);

        return new DslAdventure(
            state,
            new Dictionary<string, Location>(context.Locations, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, Item>(context.Items, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, Key>(context.Keys, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, Door>(context.Doors, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, string>(context.Metadata, StringComparer.OrdinalIgnoreCase),
            context.Warnings);
    }

    private static Location ResolveStartLocation(AdventureDslContext context)
    {
        if (!string.IsNullOrWhiteSpace(context.StartLocationId) &&
            context.Locations.TryGetValue(context.StartLocationId, out Location? start))
        {
            return start;
        }

        Location? firstLocation = context.Locations.Values.FirstOrDefault();
        return firstLocation ?? throw new InvalidOperationException("No locations defined in DSL.");
    }

    private static void ResolveDoorKeys(AdventureDslContext context)
    {
        foreach (PendingDoorKey pending in context.PendingDoorKeys)
        {
            if (!context.Doors.TryGetValue(pending.DoorId, out Door? door))
            {
                context.AddWarning(new DslParseError(
                    pending.LineNumber,
                    pending.LineContent.Trim(),
                    $"Door '{pending.DoorId}' referenced but not defined",
                    "Add a 'door:' line to define it"));
                continue;
            }

            if (!context.Keys.TryGetValue(pending.KeyId, out Key? key))
            {
                context.AddWarning(new DslParseError(
                    pending.LineNumber,
                    pending.LineContent.Trim(),
                    $"Key '{pending.KeyId}' referenced by door '{pending.DoorId}' but not defined",
                    "Add a 'key:' line to define it"));
                continue;
            }

            _ = door.RequiresKey(key);
        }
    }

    private static void ResolveExits(AdventureDslContext context)
    {
        foreach (PendingExit pending in context.PendingExits)
        {
            if (!context.Locations.TryGetValue(pending.FromId, out Location? from))
                continue;

            if (!context.Locations.ContainsKey(pending.TargetId))
            {
                context.AddWarning(new DslParseError(
                    pending.LineNumber,
                    pending.LineContent.Trim(),
                    $"Exit target '{pending.TargetId}' is not defined as a location",
                    "Add a 'location:' line to define it, or check for typos"));
            }

            Location target = context.GetOrCreateLocation(pending.TargetId);

            if (!string.IsNullOrWhiteSpace(pending.DoorId) && !context.Doors.ContainsKey(pending.DoorId))
            {
                context.AddWarning(new DslParseError(
                    pending.LineNumber,
                    pending.LineContent.Trim(),
                    $"Exit references undefined door '{pending.DoorId}'",
                    "Add a 'door:' line to define it, or check for typos"));
            }

            _ = !string.IsNullOrWhiteSpace(pending.DoorId) &&
                context.Doors.TryGetValue(pending.DoorId, out Door? door)
                ? from.AddExit(pending.Direction, target, door, pending.IsOneWay)
                : from.AddExit(pending.Direction, target, pending.IsOneWay);
        }
    }

    private readonly record struct DslItemParts(
        string Id,
        string Name,
        string? Description,
        IReadOnlyDictionary<string, string> Options);

    private readonly record struct DslDoorParts(
        string Id,
        string Name,
        string? Description,
        IReadOnlyDictionary<string, string> Options);
}

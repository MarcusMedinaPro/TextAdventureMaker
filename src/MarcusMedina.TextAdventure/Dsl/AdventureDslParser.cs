// <copyright file="AdventureDslParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace MarcusMedina.TextAdventure.Dsl;

using System.Globalization;
using MarcusMedina.TextAdventure.Engine;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Extensions;
using MarcusMedina.TextAdventure.Models;

public class AdventureDslParser : IDslParser
{
    private readonly Dictionary<string, Action<AdventureDslContext, string>> _handlers =
        new(StringComparer.OrdinalIgnoreCase);

    public AdventureDslParser() => RegisterDefaultKeywords();

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
        var context = CreateContext();

        OnBeforeParse(context);

        foreach (var line in File.ReadLines(path))
        {
            var parsedLine = ParseLine(line);
            if (parsedLine == null)
                continue;
            Dispatch(context, parsedLine.Value.Keyword, parsedLine.Value.Value);
        }

        OnAfterParse(context);

        return BuildAdventure(context);
    }

    protected virtual AdventureDslContext CreateContext() => new();

    protected virtual void OnBeforeParse(AdventureDslContext context)
    {
    }

    protected virtual void OnAfterParse(AdventureDslContext context)
    {
    }

    protected virtual (string Keyword, string Value)? ParseLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return null;
        var trimmed = line.Trim();
        if (trimmed.StartsWith("#", StringComparison.Ordinal))
            return null;
        if (trimmed.StartsWith("//", StringComparison.Ordinal))
            return null;

        var separatorIndex = trimmed.IndexOf(':');
        if (separatorIndex <= 0)
            return null;

        var keyword = trimmed[..separatorIndex].Trim();
        var value = trimmed[(separatorIndex + 1)..].Trim();
        return keyword.Length == 0 ? null : (keyword, value);
    }

    private void Dispatch(AdventureDslContext context, string keyword, string value)
    {
        if (_handlers.TryGetValue(keyword, out var handler))
        {
            handler(context, value);
        }
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
    }

    private static void HandleLocation(AdventureDslContext context, string value)
    {
        var parts = SplitParts(value);
        if (parts.Count == 0)
            return;

        var id = NormalizeId(parts[0]);
        var description = parts.Count > 1 ? parts[1] : null;
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

        var parsed = ParseItemParts(value);
        var item = context.GetOrCreateItem(parsed.Id, parsed.Name, parsed.Description);

        ApplyItemOptions(item, parsed.Options);
        context.CurrentLocation!.AddItem(item);
    }

    private static void HandleKey(AdventureDslContext context, string value)
    {
        context.RequireCurrentLocation();

        var parsed = ParseItemParts(value);
        var key = context.GetOrCreateKey(parsed.Id, parsed.Name, parsed.Description);

        ApplyItemOptions(key, parsed.Options);
        context.CurrentLocation!.AddItem(key);
    }

    private static void HandleDoor(AdventureDslContext context, string value)
    {
        var parsed = ParseDoorParts(value);
        var door = context.GetOrCreateDoor(parsed.Id, parsed.Name, parsed.Description);

        foreach (var option in parsed.Options)
        {
            if (option.Key.TextCompare("key"))
            {
                var keyId = NormalizeId(option.Value);
                if (context.Keys.TryGetValue(keyId, out var key))
                {
                    _ = door.RequiresKey(key);
                }
                else
                {
                    context.PendingDoorKeys.Add(new PendingDoorKey(parsed.Id, keyId));
                }
            }
        }
    }

    private static void HandleExit(AdventureDslContext context, string value)
    {
        context.RequireCurrentLocation();

        var parts = SplitParts(value);
        if (parts.Count == 0)
            return;

        var route = parts[0];
        var arrowIndex = route.IndexOf("->", StringComparison.Ordinal);
        if (arrowIndex <= 0)
            return;

        var directionText = route[..arrowIndex].Trim();
        var targetText = route[(arrowIndex + 2)..].Trim();

        if (!TryParseDirection(directionText, out var direction))
            return;
        var targetId = NormalizeId(targetText);

        string? doorId = null;
        var oneWay = false;

        for (var i = 1; i < parts.Count; i++)
        {
            var option = ParseOption(parts[i]);
            if (option == null)
                continue;

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
            oneWay));
    }

    private static DslItemParts ParseItemParts(string value)
    {
        var parts = SplitParts(value);
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

        var options = ParseOptions(parts, optionStart);
        return new DslItemParts(id, name, description, options);
    }

    private static DslDoorParts ParseDoorParts(string value)
    {
        var parts = SplitParts(value);
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

        var options = ParseOptions(parts, optionStart);
        return new DslDoorParts(id, name, description, options);
    }

    private static void ApplyItemOptions(Item item, IReadOnlyDictionary<string, string> options)
    {
        foreach (var option in options)
        {
            if (option.Key.TextCompare("weight") &&
                float.TryParse(option.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var weight))
            {
                _ = item.SetWeight(weight);
            }
            else if (option.Key.TextCompare("aliases"))
            {
                var aliases = option.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                _ = item.AddAliases(aliases);
            }
            else if (option.Key.TextCompare("takeable") && bool.TryParse(option.Value, out var takeable))
            {
                _ = item.SetTakeable(takeable);
            }
        }
    }

    private static void ApplyItemOptions(Key key, IReadOnlyDictionary<string, string> options)
    {
        foreach (var option in options)
        {
            if (option.Key.TextCompare("weight") &&
                float.TryParse(option.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var weight))
            {
                _ = key.SetWeight(weight);
            }
            else if (option.Key.TextCompare("aliases"))
            {
                var aliases = option.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                _ = key.AddAliases(aliases);
            }
            else if (option.Key.TextCompare("takeable") && bool.TryParse(option.Value, out var takeable))
            {
                _ = key.SetTakeable(takeable);
            }
        }
    }

    private static IReadOnlyDictionary<string, string> ParseOptions(IReadOnlyList<string> parts, int startIndex)
    {
        if (startIndex >= parts.Count)
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (var i = startIndex; i < parts.Count; i++)
        {
            var option = ParseOption(parts[i]);
            if (option == null)
                continue;
            options[option.Value.Key] = option.Value.Value;
        }

        return options;
    }

    private static KeyValuePair<string, string>? ParseOption(string raw)
    {
        var trimmed = raw.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            return null;

        var equalsIndex = trimmed.IndexOf('=');
        if (equalsIndex <= 0)
        {
            return new KeyValuePair<string, string>(trimmed, "");
        }

        var key = trimmed[..equalsIndex].Trim();
        var value = trimmed[(equalsIndex + 1)..].Trim();
        return new KeyValuePair<string, string>(key, value);
    }

    private static List<string> SplitParts(string value) => value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

    private static string NormalizeId(string value) => value.Trim().ToId();

    private static bool TryParseDirection(string value, out Direction direction) => Enum.TryParse(value, true, out direction) || value.ToLowerInvariant() switch
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

    private static bool AssignDirection(Direction direction, out Direction output, bool success = true)
    {
        output = direction;
        return success;
    }

    private static DslAdventure BuildAdventure(AdventureDslContext context)
    {
        ResolveDoorKeys(context);
        ResolveExits(context);

        var startLocation = ResolveStartLocation(context);
        var locations = context.Locations.Values.ToList();
        var state = new GameState(startLocation, worldLocations: locations);

        return new DslAdventure(
            state,
            new Dictionary<string, Location>(context.Locations, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, Item>(context.Items, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, Key>(context.Keys, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, Door>(context.Doors, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, string>(context.Metadata, StringComparer.OrdinalIgnoreCase));
    }

    private static Location ResolveStartLocation(AdventureDslContext context)
    {
        if (!string.IsNullOrWhiteSpace(context.StartLocationId) &&
            context.Locations.TryGetValue(context.StartLocationId, out var start))
        {
            return start;
        }

        var firstLocation = context.Locations.Values.FirstOrDefault();
        return firstLocation ?? throw new InvalidOperationException("No locations defined in DSL.");
    }

    private static void ResolveDoorKeys(AdventureDslContext context)
    {
        foreach (var pending in context.PendingDoorKeys)
        {
            if (!context.Doors.TryGetValue(pending.DoorId, out var door))
                continue;
            if (!context.Keys.TryGetValue(pending.KeyId, out var key))
                continue;
            _ = door.RequiresKey(key);
        }
    }

    private static void ResolveExits(AdventureDslContext context)
    {
        foreach (var pending in context.PendingExits)
        {
            if (!context.Locations.TryGetValue(pending.FromId, out var from))
                continue;

            var target = context.GetOrCreateLocation(pending.TargetId);
            _ = !string.IsNullOrWhiteSpace(pending.DoorId) &&
                context.Doors.TryGetValue(pending.DoorId, out var door)
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

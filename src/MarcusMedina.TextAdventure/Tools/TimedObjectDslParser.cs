// <copyright file="TimedObjectDslParser.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.RegularExpressions;
using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Models;

namespace MarcusMedina.TextAdventure.Tools;

public sealed class TimedObjectDslParser
{
    private static readonly Regex TimedSpawnRegex = new(
        @"timed_spawn\s+""(?<id>[^""]+)""\s*\{(?<body>[^}]*)\}",
        RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private static readonly Regex TimedDoorRegex = new(
        @"timed_door\s+""(?<id>[^""]+)""\s+direction:\s*(?<dir>\w+)\s*\{(?<body>[^}]*)\}",
        RegexOptions.IgnoreCase | RegexOptions.Multiline);

    private static readonly Regex KeyValueRegex = new(
        @"(?<key>\w+)\s*:\s*(?<value>[^;\n]+)",
        RegexOptions.IgnoreCase | RegexOptions.Multiline);

    public void Apply(string dsl, Location location)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dsl);
        ArgumentNullException.ThrowIfNull(location);

        foreach (Match match in TimedSpawnRegex.Matches(dsl))
        {
            string id = match.Groups["id"].Value.Trim();
            string body = match.Groups["body"].Value;
            TimedSpawn spawn = location.AddTimedSpawn(id);
            ApplyTimedSpawnBody(spawn, body);
        }

        foreach (Match match in TimedDoorRegex.Matches(dsl))
        {
            string id = match.Groups["id"].Value.Trim();
            string directionToken = match.Groups["dir"].Value.Trim();
            string body = match.Groups["body"].Value;

            if (!Enum.TryParse(directionToken, true, out Direction direction))
            {
                throw new InvalidOperationException($"Unknown direction '{directionToken}'.");
            }

            if (!location.Exits.TryGetValue(direction, out Exit? exit))
            {
                throw new InvalidOperationException($"No exit found for direction '{direction}'.");
            }

            TimedDoor timedDoor = exit.WithTimedDoor(id);
            ApplyTimedDoorBody(timedDoor, body);
        }
    }

    private static void ApplyTimedSpawnBody(TimedSpawn spawn, string body)
    {
        foreach ((string key, string value) in ParseKeyValues(body))
        {
            switch (key)
            {
                case "appears_at":
                    ApplyTickOrPhase(value, tick => spawn.AppearsAt(tick), phase => spawn.AppearsAt(phase));
                    break;
                case "disappears_after":
                    if (int.TryParse(value, out int ticks))
                        _ = spawn.DisappearsAfter(ticks);
                    break;
                case "disappears_at":
                    if (TryParsePhase(value, out TimePhase phase))
                        _ = spawn.DisappearsAt(phase);
                    break;
                case "message":
                    _ = spawn.Message(StripQuotes(value));
                    break;
            }
        }
    }

    private static void ApplyTimedDoorBody(TimedDoor door, string body)
    {
        foreach ((string key, string value) in ParseKeyValues(body))
        {
            switch (key)
            {
                case "opens_at":
                    ApplyTickOrPhase(value, tick => door.OpensAt(tick), phase => door.OpensAt(phase));
                    break;
                case "closes_at":
                    ApplyTickOrPhase(value, tick => door.ClosesAt(tick), phase => door.ClosesAt(phase));
                    break;
                case "opens_when":
                    if (TryParsePhaseCondition(value, out TimePhase openPhase))
                        _ = door.OpensAt(openPhase);
                    break;
                case "closes_when":
                    if (TryParsePhaseCondition(value, out TimePhase closePhase))
                        _ = door.ClosesAt(closePhase);
                    break;
                case "message":
                    _ = door.Message(StripQuotes(value));
                    break;
                case "closed_message":
                    _ = door.ClosedMessage(StripQuotes(value));
                    break;
            }
        }
    }

    private static IEnumerable<(string Key, string Value)> ParseKeyValues(string body)
    {
        foreach (Match match in KeyValueRegex.Matches(body))
        {
            string key = match.Groups["key"].Value.Trim().ToLowerInvariant();
            string value = match.Groups["value"].Value.Trim();
            yield return (key, value);
        }
    }

    private static void ApplyTickOrPhase(string value, Action<int> onTick, Action<TimePhase> onPhase)
    {
        value = StripQuotes(value);
        if (int.TryParse(value, out int tick))
        {
            onTick(tick);
            return;
        }

        if (TryParsePhase(value, out TimePhase phase))
        {
            onPhase(phase);
        }
    }

    private static bool TryParsePhaseCondition(string value, out TimePhase phase)
    {
        value = value.Replace("time_phase", "", StringComparison.OrdinalIgnoreCase)
            .Replace("==", "", StringComparison.OrdinalIgnoreCase)
            .Trim();
        return TryParsePhase(StripQuotes(value), out phase);
    }

    private static bool TryParsePhase(string value, out TimePhase phase)
    {
        return Enum.TryParse(value, true, out phase);
    }

    private static string StripQuotes(string value)
    {
        value = value.Trim();
        if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
        {
            return value[1..^1];
        }

        return value;
    }
}

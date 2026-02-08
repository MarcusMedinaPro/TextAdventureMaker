// <copyright file="LocationExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;
using MarcusMedina.TextAdventure.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MarcusMedina.TextAdventure.Extensions;

public static class LocationExtensions
{
    private static readonly Regex DslItemRegex = new(
        @"^(?<name>[\w\s]+)(\((?<props>[^)]+)\))?(\s*\|\s*(?<desc>.+))?$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static List<string> GetRoomItems(this ILocation location, bool showAll = false, ILanguageProvider? provider = null)
    {
        ArgumentNullException.ThrowIfNull(location);
        provider ??= Language.Provider;

        return [.. location.Items
            .Where(item => showAll || !item.HiddenFromItemList)
            .Select(item => GetName(provider, item))];
    }

    public static List<string> GetRoomExits(this ILocation location, ILanguageProvider? provider = null)
    {
        ArgumentNullException.ThrowIfNull(location);
        provider ??= Language.Provider;

        return [.. location.Exits.Keys.Select(direction => GetDirectionName(provider, direction))];
    }

    public static FlashbackLocationBuilder TriggerFlashback(this ILocation location, string memoryId)
    {
        ArgumentNullException.ThrowIfNull(location);
        if (location is not Location concrete)
        {
            throw new InvalidOperationException("Flashbacks require a concrete Location instance.");
        }

        FlashbackTrigger trigger = concrete.AddFlashbackTrigger(memoryId);
        return new FlashbackLocationBuilder(trigger);
    }

    public static ILocation AddItems(this ILocation location, params string[] itemNames)
    {
        ArgumentNullException.ThrowIfNull(location);
        if (itemNames == null)
        {
            return location;
        }

        foreach (string name in itemNames)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            Item item = new(name.ToId(), name.Trim());
            location.AddItem(item);
        }

        return location;
    }

    public static ILocation AddDSLItems(this ILocation location, params string[] entries)
    {
        ArgumentNullException.ThrowIfNull(location);
        if (entries == null)
        {
            return location;
        }

        foreach (string entry in entries)
        {
            Item? item = ParseDslItem(entry);
            if (item != null)
            {
                location.AddItem(item);
            }
        }

        return location;
    }

    private static Item? ParseDslItem(string entry)
    {
        if (string.IsNullOrWhiteSpace(entry))
        {
            return null;
        }

        Match match = DslItemRegex.Match(entry.Trim());
        if (!match.Success)
        {
            return null;
        }

        string name = match.Groups["name"].Value.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        Item item = new(name.ToId(), name);
        string props = match.Groups["props"].Value;
        string desc = match.Groups["desc"].Value;

        if (!string.IsNullOrWhiteSpace(desc))
        {
            _ = item.SetDescription(desc.Trim());
        }

        bool? takeable = null;
        if (!string.IsNullOrWhiteSpace(props))
        {
            foreach (string raw in props.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                string token = raw.Trim();
                if (token.Equals("takeable", StringComparison.OrdinalIgnoreCase))
                {
                    takeable = true;
                    continue;
                }

                if (token.Equals("fixed", StringComparison.OrdinalIgnoreCase))
                {
                    takeable = false;
                    continue;
                }

                string weightToken = token.Replace("kg", "", StringComparison.OrdinalIgnoreCase);
                if (float.TryParse(weightToken, NumberStyles.Float, CultureInfo.InvariantCulture, out float weight))
                {
                    _ = item.SetWeight(weight);
                }
            }
        }

        if (takeable.HasValue)
        {
            _ = item.SetTakeable(takeable.Value);
        }

        return item;
    }

    private static string GetName(ILanguageProvider provider, IGameEntity entity)
    {
        return provider is JsonLanguageProvider json ? json.GetName(entity.Id) : entity.Name;
    }

    private static string GetDirectionName(ILanguageProvider provider, Direction direction)
    {
        return provider is JsonLanguageProvider json ? json.GetDirectionName(direction) : direction.ToString();
    }
}

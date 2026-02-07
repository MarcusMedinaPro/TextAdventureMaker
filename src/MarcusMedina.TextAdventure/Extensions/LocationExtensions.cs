// <copyright file="LocationExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;
using MarcusMedina.TextAdventure.Localization;

namespace MarcusMedina.TextAdventure.Extensions;

public static class LocationExtensions
{
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

    private static string GetName(ILanguageProvider provider, IGameEntity entity)
    {
        return provider is JsonLanguageProvider json ? json.GetName(entity.Id) : entity.Name;
    }

    private static string GetDirectionName(ILanguageProvider provider, Direction direction)
    {
        return provider is JsonLanguageProvider json ? json.GetDirectionName(direction) : direction.ToString();
    }
}

// <copyright file="GameEntityExtensions.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Extensions;

using MarcusMedina.TextAdventure.Interfaces;

public static class GameEntityExtensions
{
    /// <summary>
    /// Gets a hint for a game entity.
    /// </summary>
    public static string? GetHint(this IPropertyBag entity) => entity.GetProperty("hint");

    /// <summary>
    /// Gets an arbitrary string property from a property bag.
    /// </summary>
    public static string? GetProperty(this IPropertyBag entity, string key)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return string.IsNullOrWhiteSpace(key) ? null : entity.Properties.TryGetValue(key.Trim(), out var value) ? value : null;
    }

    /// <summary>
    /// Gets a typed property value using IParsable, returning a default if missing or unparseable.
    /// </summary>
    public static T GetProperty<T>(this IPropertyBag entity, string key, T defaultValue) where T : IParsable<T>
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (string.IsNullOrWhiteSpace(key))
            return defaultValue;
        if (!entity.Properties.TryGetValue(key.Trim(), out var raw) || string.IsNullOrWhiteSpace(raw))
            return defaultValue;
        return T.TryParse(raw, null, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Gets a boolean property, treating "true", "1", and "yes" as true.
    /// </summary>
    public static bool GetBoolProperty(this IPropertyBag entity, string key, bool defaultValue = false)
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (string.IsNullOrWhiteSpace(key))
            return defaultValue;
        if (!entity.Properties.TryGetValue(key.Trim(), out var raw) || string.IsNullOrWhiteSpace(raw))
            return defaultValue;
        return raw.Lower() is "true" or "1" or "yes";
    }

    /// <summary>
    /// Sets a hint for a property bag entity.
    /// </summary>
    public static T SetHint<T>(this T entity, string text) where T : IPropertyBag => entity.SetProperty("hint", text);

    /// <summary>
    /// Sets an arbitrary string property on any property bag entity.
    /// </summary>
    public static T SetProperty<T>(this T entity, string key, string value) where T : IPropertyBag
    {
        ArgumentNullException.ThrowIfNull(entity);
        if (string.IsNullOrWhiteSpace(key))
            return entity;

        entity.Properties[key.Trim()] = value ?? "";
        return entity;
    }
}
